using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.SQLServerDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace DataManagerAPI.PostgresDB.Implementation;

/// <summary>
/// Implementation of <see cref="IUserFilesRepository"/> for Postgres database.
/// </summary>
public class UserFileRepositoryPostgres : IUserFilesRepository
{
    private readonly PostgresDBContext _context;
    private readonly ILogger<UserFileRepositoryPostgres> _logger;

    private readonly bool _useBufferingForBigFiles;     // flag for using buffering for big files
    private readonly bool _useBufferingForSmallFiles;   // flag for using buffering for "small" files

    // it is used if _useBufferingForBigFiles or _useBufferingForSmallFiles are "true".
    private readonly int _bufferSizeForStreamCopy = 1024 * 1024 * 4;    // default size of the buffer 4 MB

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context">Database context. <see cref="UsersDBContext"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="logger"></param>
    public UserFileRepositoryPostgres(UsersDBContext context, IConfiguration configuration, ILogger<UserFileRepositoryPostgres> logger)
    {
        _context = (context as PostgresDBContext)!;
        _logger = logger;

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
        _logger.LogInformation("Started:{userDataId},{fileId}", userDataId, fileId);

        var result = new ResultWrapper<int>
        {
            Success = true,
            Data = fileId
        };

        try
        {
            await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            var request = $"""SELECT "Oid" FROM "UserFiles" WHERE "Id"={fileId} AND "UserDataId"={userDataId}""";
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
                Helpers.LogNotFoundWarning(result, $"File {fileId} in UserData {userDataId} not found", _logger);
                return result;  // nothing to delete
            }

            if (oid.Value != 0) // delete BLOB
            {
                _logger.LogDebug("Deleting BLOB");
                var manager = new NpgsqlLargeObjectManager(connection);
                await manager.UnlinkAsync((uint)oid!, cancellationToken);
            }

            _logger.LogDebug("Deleting record");

            // delete record
            command.CommandText = $"""DELETE FROM "UserFiles" WHERE "Id"={fileId} AND "UserDataId"={userDataId}""";
            _ = await command.ExecuteScalarAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    private class FileInfo
    {
        public long FileSize { get; set; }
        public uint Oid { get; set; }
        public string FileName { get; set; } = string.Empty;
    }

    private async Task<ResultWrapper<FileInfo>> GetFileInformation(int userDataId, int fileId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var result = new ResultWrapper<FileInfo>();

        try
        {
            await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            var request = $"""SELECT "Oid", "Size", "Name" FROM "UserFiles" WHERE "Id"={fileId} AND "UserDataId"={userDataId}""";

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
                result.Success = false;
                result.Message = $"File {fileId} in UserData {userDataId} not found";
                result.StatusCode = ResultStatusCodes.Status404NotFound;
                return result;
            }

            result.Success = true;
            result.Data = new FileInfo { FileSize = fileSize ?? 0, Oid = oid ?? 0, FileName = fileName ?? string.Empty };
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished:{@data}", result.Data);

        return result;
    }

    private async Task<ResultWrapper<UserFileStream>> DownloadSmallFile(int userDataId, int fileId, FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var result = new ResultWrapper<UserFileStream>();

        await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
        await connection.OpenAsync(cancellationToken);

        using var command = new NpgsqlCommand(
            $"""SELECT "Data" FROM "UserFiles" WHERE "Id"={fileId}""",
            connection);

        _logger.LogDebug("Reading data from database into memory");

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

        _logger.LogInformation("Finished");

        return result;
    }

    private async Task<ResultWrapper<UserFileStream>> DownloadBigFile(int userDataId, int fileId, FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var result = new ResultWrapper<UserFileStream>();

        _logger.LogDebug("Opening output stream");

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

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileStream>> DownloadFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userDataId},{fileId}", userDataId, fileId);

        var result = new ResultWrapper<UserFileStream>();

        ResultWrapper<FileInfo> info = await GetFileInformation(userDataId, fileId, cancellationToken);
        if (!info.Success)
        {
            result.StatusCode = info.StatusCode;
            result.Message = info.Message;

            _logger.LogInformation("Finished");

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
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFile[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userDataId}", userDataId);

        var result = new ResultWrapper<UserFile[]>
        {
            Success = true
        };

        try
        {
            var userData = await FindUserData<UserFile[]>(userDataId, cancellationToken);
            if (!userData.Success)  // there is no UserData item
            {
                return userData;
            }

            result.Data = await _context.UserFiles.Where(x => x.UserDataId == userDataId).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFile>> UploadFileAsync(UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userDataId},{fileId},{name}", fileStream.UserDataId, fileStream.Id, fileStream.Name);

        var result = new ResultWrapper<UserFile>
        {
            Success = true
        };

        try
        {
            ResultWrapper<FileInfo> info = await GetFileInformation(fileStream.UserDataId, fileStream.Id, cancellationToken);

            if (info.Data == null && fileStream.Id != 0) // file doesn't exist, but Id is not 0.
            {
                result.Success = false;
                result.Message = "FileId not equal 0 is not allowed for new file";
                result.StatusCode = ResultStatusCodes.Status400BadRequest;
                _logger.LogWarning("Finished:{@result}", result);

                return result;
            }

            await using var connection = new NpgsqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            if (fileStream.BigFile)
            {
                await ProcessBigFile(info.Data, connection, fileStream, cancellationToken);
            }
            else
            {
                await ProcessFile(info.Data, connection, fileStream, cancellationToken);
            }

            result.Data = new UserFile { UserDataId = fileStream.UserDataId, Id = fileStream.Id, Name = fileStream.Name, Size = fileStream.Content!.Length };
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished:{@data}", result.Data);

        return result;
    }


    private async Task ProcessFile(FileInfo? info, NpgsqlConnection connection,
        UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        await using var memoryStream = new MemoryStream();

        _logger.LogDebug("Copying to memory stream");

        if (_useBufferingForSmallFiles)
        {
            byte[] buffer = new byte[_bufferSizeForStreamCopy];
            int read, count = 0;
            while ((read = await fileStream.Content!.ReadAsync(buffer, cancellationToken)) > 0)
            {
                count++;
                await memoryStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
            _logger.LogDebug("IterationsCount:{count}", count);
        }
        else
        {
            await fileStream.Content!.CopyToAsync(memoryStream, cancellationToken);
        }

        string request = string.Empty;

        _logger.LogDebug("Writing to database");

        if (info == null)   // new file
        {
            request = $"""INSERT INTO "UserFiles" ("UserDataId", "Name", "Size", "Oid", "Data")""" +
                $" VALUES({fileStream.UserDataId}, '{fileStream.Name}', {memoryStream.Length}, 0, @binaryValue)" +
                @" RETURNING ""Id""";
        }
        else
        {
            if (info.Oid != 0)
            {
                var manager = new NpgsqlLargeObjectManager(connection);

                _logger.LogDebug("Deleting BLOB");
                await manager.UnlinkAsync(info.Oid, cancellationToken);
            }

            request = $"""UPDATE "UserFiles" SET "Name"='{fileStream.Name}', "Size"={memoryStream.Length}, "Oid"=0, "Data"='@binaryValue'""" +
                $""" WHERE "Id"={fileStream.Id}""";
        }

        await using var command = new NpgsqlCommand(request, connection);
        command.CommandTimeout = 0;
        command.Parameters.AddWithValue("@binaryValue", NpgsqlTypes.NpgsqlDbType.Bytea, memoryStream.ToArray());

        var tmp = await command.ExecuteScalarAsync(cancellationToken);
        if (tmp != null)
        {
            fileStream.Id = (tmp as int?)!.Value;
        }

        _logger.LogInformation("Finished");
    }

    private async Task ProcessBigFile(FileInfo? info, NpgsqlConnection connection, UserFileStream fileStream,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var manager = new NpgsqlLargeObjectManager(connection);
        manager.MaxTransferBlockSize = _bufferSizeForStreamCopy;

        if (info != null && info.Oid != 0)   // delete existing BLOB
        {
            _logger.LogDebug("Deleting BLOB");
            await manager.UnlinkAsync(info.Oid, cancellationToken);
        }

        uint oid = await manager.CreateAsync(0, cancellationToken);    // create new BLOB

        NpgsqlLargeObjectStream? stream;

        _logger.LogDebug("Writing to database");

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        stream = await manager.OpenReadWriteAsync(oid, cancellationToken);

        if (_useBufferingForBigFiles)
        {
            byte[] buffer = new byte[_bufferSizeForStreamCopy];
            int read, count = 0;
            while ((read = await fileStream.Content!.ReadAsync(buffer, cancellationToken)) > 0)
            {
                count++;
                await stream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
            _logger.LogDebug("IterationsCount:{count}", count);
        }
        else
        {
            await fileStream.Content!.CopyToAsync(stream, cancellationToken);
        }

        _logger.LogDebug("Updating record");

        string request;

        if (info == null)   // new file
        {
            request = """INSERT INTO "UserFiles" ("UserDataId", "Name", "Size", "Oid", "Data")""" +
                $" VALUES({fileStream.UserDataId}, '{fileStream.Name}', {stream.Length}, {oid}, NULL)" +
                @" RETURNING ""Id""";
        }
        else
        {
            request = $"""UPDATE "UserFiles" SET "Name"='{fileStream.Name}', "Size"={stream.Length}, "Oid"={oid}, "Data"=NULL""" +
                $""" WHERE "Id"={fileStream.Id}""";
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

        _logger.LogInformation("Finished");
    }

    private async Task<ResultWrapper<T>> FindUserData<T>(int userDataId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<T>
        {
            Success = true
        };

        var userData = await _context.UserData.FirstOrDefaultAsync(x => x.Id == userDataId, cancellationToken);
        if (userData is null)
        {
            Helpers.LogNotFoundWarning(result, $"UserDataId {userDataId} not found", _logger);
        }

        return result;
    }

}
