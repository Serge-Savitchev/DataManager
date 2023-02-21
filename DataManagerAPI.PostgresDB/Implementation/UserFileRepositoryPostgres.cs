using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.SQLServerDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DataManagerAPI.PostgresDB.Implementation;

/// <summary>
/// Implementation of <see cref="IUserFilesRepository"/> for Postgres database.
/// </summary>
public class UserFileRepositoryPostgres : IUserFilesRepository
{
    private readonly PostgresDBContext _context;

    private readonly bool _useBufferingForBigFiles;     // flag for using buffering for big files
    private readonly bool _useBufferingForSmallFiles;   // flag for using buffering for "small" files

    // it is used if _useBufferingForBigFiles or _useBufferingForSmallFiles are "true".
    private readonly int _bufferSizeForStreamCopy = 1024 * 1024 * 4;    // default size of the buffer 4 MB

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context">Database context. <see cref="UsersDBContext"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    public UserFileRepositoryPostgres(UsersDBContext context, IConfiguration configuration)
    {
        _context = (context as PostgresDBContext)!;

        if (!bool.TryParse(configuration["Buffering:Server:UseBufferingForBigFiles"], out _useBufferingForBigFiles))
        {
            _useBufferingForBigFiles = false;
        }

        if (!bool.TryParse(configuration["Buffering:Server:UseBufferingForSmallFiles"], out _useBufferingForSmallFiles))
        {
            _useBufferingForSmallFiles = false;
        }

        if (int.TryParse(configuration["Buffering:Server:BufferSize"], out int size) && size > 0)
        {
            _bufferSizeForStreamCopy = size * 1024;
        }

    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> DeleteFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<int>
        {
            Success = true,
            Data = fileId
        };

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync(cancellationToken);

        var request = $"SELECT \"Oid\" FROM \"UserFiles\" WHERE \"Id\"={fileId} AND \"UserDataId\"={userDataId}";
        await using var command = new NpgsqlCommand(request, connection);

        uint? oid = null;

        // try to get file record by Id and UserDataId
        await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            if (await reader.ReadAsync(cancellationToken))
            {
                oid = reader.IsDBNull(0) ? null : (uint)reader.GetInt64(0);
            }
        }

        if (oid == null)    // there is no such record
        {
            return result;  // nothing to delete
        }

        if (oid.Value != 0) // delete BLOB
        {
            var manager = new NpgsqlLargeObjectManager(connection);
            await manager.UnlinkAsync((uint)oid!, cancellationToken);
        }

        // delete record
        command.CommandText = $"DELETE FROM \"UserFiles\" WHERE \"Id\"={fileId} AND \"UserDataId\"={userDataId}";
        _ = await command.ExecuteScalarAsync(cancellationToken);

        return result;
    }

    private class FileInfo
    {
        public long FileSize { get; set; }
        public uint Oid { get; set; }
        public string FileName { get; set; } = string.Empty;
    }

    private async Task<ResultWrapper<FileInfo>> GetFileInformation(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<FileInfo>();

        try
        {
            await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            var request = "SELECT \"Oid\", \"Size\", \"Name\" FROM \"UserFiles\"" +
                        $" WHERE \"Id\"={fileId} AND \"UserDataId\"={userDataId}";

            await using var command = new NpgsqlCommand(request, connection);

            uint? oid = null;
            long? fileSize = null;
            string? fileName = null;

            await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    oid = reader.IsDBNull(0) ? null : (uint)reader.GetInt64(0);
                    fileSize = reader.IsDBNull(1) ? null : reader.GetInt64(1);
                    fileName = reader.IsDBNull(2) ? null : reader.GetString(2);
                }
            }

            if (fileSize == null || fileSize <= 0 || string.IsNullOrWhiteSpace(fileName))
            {
                result.StatusCode = StatusCodes.Status404NotFound;
            }
            else
            {
                result.Success = true;
                result.Data = new FileInfo { FileSize = fileSize ?? 0, Oid = oid ?? 0, FileName = fileName ?? string.Empty };
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    private async Task<ResultWrapper<UserFileStream>> DownloadSmallFile(int userDataId, int fileId, FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserFileStream>();

        try
        {
            await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            using var command = new NpgsqlCommand(
                $"SELECT \"Data\" FROM \"UserFiles\" WHERE \"Id\"={fileId}",
                connection);

            command.CommandTimeout = 0;

            byte[] data = new byte[fileInfo.FileSize];

            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            await reader.ReadAsync(cancellationToken);

            fileInfo.FileSize = reader.GetBytes(0, 0, data, 0, (int)fileInfo.FileSize);

            var outStream = new MemoryStream(data);

            result.Data = new UserFileStream
            {
                Id = fileId,
                UserDataId = userDataId,
                Name = fileInfo.FileName,
                Size = fileInfo.FileSize,
                Content = new BufferedStream(outStream, _bufferSizeForStreamCopy)
            };

            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    private async Task<ResultWrapper<UserFileStream>> DownloadBigFile(int userDataId, int fileId, FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserFileStream>();

        var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync(cancellationToken);

        var manager = new NpgsqlLargeObjectManager(connection);
        manager.MaxTransferBlockSize = _bufferSizeForStreamCopy;

        _ = await connection.BeginTransactionAsync(cancellationToken);
        NpgsqlLargeObjectStream stream = await manager.OpenReadAsync(fileInfo.Oid, cancellationToken);

        result.Data = new UserFileStream
        {
            Id = fileId,
            UserDataId = userDataId,
            Name = fileInfo.FileName,
            Size = fileInfo.FileSize,
            Content = new BufferedStream(stream, _bufferSizeForStreamCopy)
        };

        result.Success = true;

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileStream>> DownloadFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserFileStream>();

        ResultWrapper<FileInfo> info = await GetFileInformation(userDataId, fileId, cancellationToken);
        if (!info.Success)
        {
            result.StatusCode = info.StatusCode;
            result.Message = info.Message;
            return result;
        }

        try
        {
            if (info.Data!.Oid != 0)    // big file
            {
                return await DownloadBigFile(userDataId, fileId, info.Data, cancellationToken);
            }
            else
            {
                return await DownloadSmallFile(userDataId, fileId, info.Data, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFile[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserFile[]>
        {
            Success = true
        };

        var userData = await FindUserData<UserFile[]>(userDataId, cancellationToken);
        if (!userData.Success)  // there is no UserData for the file
        {
            return userData;
        }

        try
        {
            result.Data = await _context.UserFiles.Where(x => x.UserDataId == userDataId).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFile>> UploadFileAsync(UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        ResultWrapper<UserFile> userData = await FindUserData<UserFile>(fileStream.UserDataId, cancellationToken);
        if (!userData.Success)     // there is no UserData for the file
        {
            return userData;
        }

        var result = new ResultWrapper<UserFile>
        {
            Success = true
        };

        try
        {
            await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            var request = $"SELECT \"Name\" FROM \"UserFiles\" WHERE \"Id\"={fileStream.Id} AND \"UserDataId\"={fileStream.UserDataId}";
            await using var command = new NpgsqlCommand(request, connection);

            // take file name from DB. null value means that record doesn't exist.
            var fileName = await command.ExecuteScalarAsync(cancellationToken) as string;
            if (fileName == null && fileStream.Id != 0) // file doesn't exist, but Id is not 0.
            {
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Success = false;
                return result;
            }

            if (fileStream.BigFile)
            {
                await ProcessBigFile(fileName == null, connection, fileStream, cancellationToken);
            }
            else
            {
                await ProcessFile(fileName == null, connection, fileStream, cancellationToken);
            }

            result.Data = new UserFile { UserDataId = fileStream.UserDataId, Id = fileStream.Id, Name = fileStream.Name, Size = fileStream.Content!.Length };
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }


    private async Task ProcessFile(bool newFile, NpgsqlConnection connection,
        UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        await using var memoryStream = new MemoryStream();

        if (_useBufferingForSmallFiles)
        {
            byte[] buffer = new byte[_bufferSizeForStreamCopy];
            int read, count = 0;
            while ((read = await fileStream.Content!.ReadAsync(buffer, cancellationToken)) > 0)
            {
                Console.WriteLine($"{++count}\t{read}");
                await memoryStream.WriteAsync(buffer, cancellationToken);
            }
        }
        else
        {
            await fileStream.Content!.CopyToAsync(memoryStream, cancellationToken);
        }

        string request = string.Empty;

        if (newFile)   // new file
        {
            request = $"INSERT INTO \"UserFiles\" (\"UserDataId\", \"Name\", \"Size\", \"Oid\", \"Data\")" +
                $" VALUES({fileStream.UserDataId}, '{fileStream.Name}', {memoryStream.Length}, 0, @binaryValue)" +
                $" RETURNING \"Id\"";
        }
        else
        {
            request = $"UPDATE \"UserFiles\" SET \"Name\"='{fileStream.Name}', \"Size\"={memoryStream.Length}, \"Data\"='@binaryValue'" +
                $" WHERE \"Id\"={fileStream.Id}";
        }

        await using var command = new NpgsqlCommand(request, connection);
        command.CommandTimeout = 0;
        command.Parameters.AddWithValue("@binaryValue", NpgsqlTypes.NpgsqlDbType.Bytea, memoryStream.ToArray());

        var tmp = await command.ExecuteScalarAsync(cancellationToken);
        if (tmp != null)
        {
            fileStream.Id = (tmp as int?)!.Value;
        }
    }

    private async Task ProcessBigFile(bool newFile, NpgsqlConnection connection, UserFileStream fileStream,
        CancellationToken cancellationToken = default)
    {
        uint? oid = null;

        await using (var command = new NpgsqlCommand(null, connection))
        {
            command.CommandText = $"SELECT \"Oid\" FROM \"UserFiles\" WHERE \"Id\"={fileStream.Id} AND \"UserDataId\"={fileStream.UserDataId}";

            using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    oid = reader.IsDBNull(0) ? null : (uint)reader.GetInt64(0);
                }
            }
        }

        var manager = new NpgsqlLargeObjectManager(connection);
        manager.MaxTransferBlockSize = _bufferSizeForStreamCopy;

        if (oid != null && oid.Value != 0)   // delete existing BLOB
        {
            await manager.UnlinkAsync(oid.Value, cancellationToken);
        }

        oid = await manager.CreateAsync(0, cancellationToken);    // create new BLOB

        NpgsqlLargeObjectStream? stream = null;

        await using (var transaction = await connection.BeginTransactionAsync(cancellationToken))
        {
            stream = await manager.OpenReadWriteAsync(oid.Value, cancellationToken);

            if (_useBufferingForBigFiles)
            {
                byte[] buffer = new byte[_bufferSizeForStreamCopy];
                int read, count = 0;
                while ((read = await fileStream.Content!.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    Console.WriteLine($"{++count}\t{read}");
                    await stream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                }
            }
            else
            {
                await fileStream.Content!.CopyToAsync(stream, cancellationToken);
            }

            string request;

            if (newFile)   // new file
            {
                request = $"INSERT INTO \"UserFiles\" (\"UserDataId\", \"Name\", \"Size\", \"Oid\", \"Data\")" +
                    $" VALUES({fileStream.UserDataId}, '{fileStream.Name}', {stream.Length}, {oid}, NULL)" +
                    $" RETURNING \"Id\"";
            }
            else
            {
                request = $"UPDATE \"UserFiles\" SET \"Name\"='{fileStream.Name}', \"Size\"={stream.Length}, \"Oid\"={oid}, \"Data\"=NULL" +
                    $" WHERE \"Id\"={fileStream.Id}";
            }

            await using var cmd = new NpgsqlCommand(request, connection);
            cmd.CommandTimeout = 0;
            cmd.CommandText = request;

            var tmp = await cmd.ExecuteScalarAsync(cancellationToken);
            if (tmp != null)
            {
                fileStream.Id = (tmp as int?)!.Value;
            }

            transaction.Commit();
        }

    }

    private async Task<ResultWrapper<T>> FindUserData<T>(int userDataId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<T>
        {
            Success = true
        };

        try
        {
            var userData = await _context.UserData.FirstOrDefaultAsync(x => x.Id == userDataId, cancellationToken);
            if (userData is null)
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            result.Success = false;
            result.StatusCode = StatusCodes.Status404NotFound;
            result.Message = $"UserDataId {userDataId} not found";
        }

        return result;
    }

}
