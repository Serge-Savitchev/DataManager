using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DataManagerAPI.SQLServerDB.Implementation;

/// <summary>
/// Implementation of <see cref="IUserFilesRepository"/>.
/// </summary>
public class UserFilesRepository : IUserFilesRepository
{
    private readonly UsersDBContext _context;
    private readonly ILogger<UserFilesRepository> _logger;

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
    public UserFilesRepository(UsersDBContext context, IConfiguration configuration, ILogger<UserFilesRepository> logger)
    {
        _context = context;
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
            var record = await _context.UserFiles.FirstOrDefaultAsync(x => x.Id == fileId && x.UserDataId == userDataId, cancellationToken);
            if (record != null) // record exists
            {
                _logger.LogDebug("Deleting record");

                _context.UserFiles.Remove(record);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                Helpers.LogNotFoundWarning(result, $"File {fileId} in UserData {userDataId} not found", _logger);
                return result;
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
    public async Task<ResultWrapper<UserFileStream>> DownloadFileAsync(int userDataId, int fileId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userDataId},{fileId}", userDataId, fileId);

        var result = new ResultWrapper<UserFileStream>();
        try
        {
            var sqlConnection = (_context.Database.GetDbConnection() as SqlConnection)!;
            var dbName = MigrationExtensions.ExtructDBNameFromConnectionString(_context.Database.GetConnectionString()!);

            await sqlConnection.OpenAsync(cancellationToken);

            long? contentSize = null, dataSize = null, fileSize = null;
            string? fileName = null;

            var request = $"SELECT LEN(Content), LEN(Data), Size, Name FROM {dbName}.dbo.UserFiles" +
                $" WHERE Id={fileId} AND UserDataId={userDataId}";

            var sqlCommand = new SqlCommand(request, sqlConnection);

            await using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    contentSize = reader.IsDBNull(0) ? null : reader.GetInt64(0);
                    dataSize = reader.IsDBNull(1) ? null : reader.GetInt64(1);
                    fileSize = reader.IsDBNull(2) ? null : reader.GetInt64(2);
                    fileName = reader.IsDBNull(3) ? null : reader.GetString(3);
                }
            }

            if (fileSize == null || fileSize <= 0 || string.IsNullOrWhiteSpace(fileName))
            {
                Helpers.LogNotFoundWarning(result, $"File {fileId} in UserData {userDataId} not found", _logger);
                return result;
            }

            Stream? outStream = null;

            _logger.LogDebug("File:{fileName},Size:{fileSize},Bigfile:{bigFile}", fileName, fileSize, contentSize != null && contentSize > 0);

            if (contentSize != null && contentSize > 0)   // Big file
            {
                sqlCommand.Transaction = sqlConnection.BeginTransaction("mainTranaction");
                sqlCommand.CommandText = $"SELECT Content.PathName() FROM {dbName}.dbo.UserFiles WHERE Id={fileId}";

                string? filePath = await sqlCommand.ExecuteScalarAsync(cancellationToken) as string;

                sqlCommand.CommandText = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()";
                var txContext = (byte[])await sqlCommand.ExecuteScalarAsync(cancellationToken);

                outStream = new SqlFileStream(filePath, txContext, FileAccess.Read);
            }
            else if (dataSize != null && dataSize >= 0)
            {
                byte[] data = new byte[dataSize.Value];

                sqlCommand.CommandText = $"SELECT Data FROM {dbName}.dbo.UserFiles WHERE Id={fileId}";
                await using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);

                await reader.ReadAsync(cancellationToken);

                dataSize = reader.GetBytes(0, 0, data, 0, (int)dataSize);
                outStream = new MemoryStream(data);
            }
            else
            {
                throw new Exception($"Unexpected data. UserDataId:{userDataId} Id: {fileId}");
            }

            result.Data = new UserFileStream
            {
                Id = fileId,
                UserDataId = userDataId,
                Name = fileName,
                Size = dataSize ?? 0,
                Content = new BufferedStream(outStream, _bufferSizeForStreamCopy)
            };

            result.Success = true;
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
            ResultWrapper<UserFile> userData = await FindUserData<UserFile>(fileStream.UserDataId, cancellationToken);
            if (!userData.Success)     // there is no UserData
            {
                return userData;
            }

            var sqlConnection = (_context.Database.GetDbConnection() as SqlConnection)!;
            var dbName = MigrationExtensions.ExtructDBNameFromConnectionString(_context.Database.GetConnectionString()!);

            await sqlConnection.OpenAsync(cancellationToken);

            var sqlCommand = new SqlCommand(
                    $"SELECT Name FROM {dbName}.dbo.UserFiles WHERE Id={fileStream.Id} AND UserDataId={fileStream.UserDataId}",
                    sqlConnection);

            // take file name from DB. null value means that record doesn't exist.
            var fileName = await sqlCommand.ExecuteScalarAsync(cancellationToken) as string;

            if (fileName == null && fileStream.Id != 0) // file doesn't exist, but Id is not 0.
            {
                result.Success = false;
                result.Message = "FileId not equal 0 is not allowed for new file";
                result.StatusCode = ResultStatusCodes.Status400BadRequest;
                _logger.LogWarning("Finished:{@result}", result);

                return result;
            }

            if (fileStream.BigFile)
            {
                await ProcessBigFile(fileName, dbName, sqlConnection, fileStream, cancellationToken);
            }
            else
            {
                await ProcessFile(fileName, dbName, sqlConnection, fileStream, cancellationToken);
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

    private async Task ProcessFile(string? fileName, string dbName, SqlConnection sqlConnection,
        UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var sqlCommand = new SqlCommand("", sqlConnection);
        sqlCommand.CommandTimeout = 0;

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

        if (fileName == null)   // new file
        {
            request = $"INSERT INTO {dbName}.dbo.UserFiles (FileId, UserDataId, Name, Size, Content, Data)" +
                " OUTPUT INSERTED.[Id]" +
                $" VALUES(NEWID(), {fileStream.UserDataId}, '{fileStream.Name}', {memoryStream.Length}, NULL, @binaryValue)";
        }
        else
        {
            request = $"UPDATE {dbName}.dbo.UserFiles SET Name='{fileStream.Name}', Size={memoryStream.Length}, Content=NULL, Data=@binaryValue" +
                $" WHERE Id={fileStream.Id}";
        }

        sqlCommand.CommandText = request;
        sqlCommand.Parameters.AddWithValue("@binaryValue", memoryStream.ToArray());
        var tmp = await sqlCommand.ExecuteScalarAsync(cancellationToken);
        if (tmp != null)
        {
            fileStream.Id = (tmp as int?)!.Value;
        }

        _logger.LogInformation("Finished");
    }

    private async Task ProcessBigFile(string? fileName, string dbName, SqlConnection sqlConnection,
        UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var sqlCommand = new SqlCommand(
            $"SELECT Content.PathName() FROM {dbName}.dbo.UserFiles WHERE Id={fileStream.Id}",
            sqlConnection);

        sqlCommand.CommandTimeout = 0;

        string? filePath = await sqlCommand.ExecuteScalarAsync(cancellationToken) as string;

        if (fileName == null)   // new file
        {
            _logger.LogDebug("Inserting record into database");

            var request = $"INSERT INTO {dbName}.dbo.UserFiles (FileId, UserDataId, Name, Size, Content, Data)" +
            " OUTPUT INSERTED.[Id]" +
            $" VALUES(NEWID(), {fileStream.UserDataId}, '{fileStream.Name}', 0, 0, NULL)";

            sqlCommand.CommandText = request;
            fileStream.Id = (await sqlCommand.ExecuteScalarAsync(cancellationToken) as int?)!.Value;
        }

        if (filePath == null)  // record exists, but FILESTREAM data is empty
        {
            _logger.LogDebug("Deleting existing data");

            // create empty data
            sqlCommand.CommandText = $"UPDATE {dbName}.dbo.UserFiles SET Size=0, Content=0, Data=NULL," +
                $" Name='{fileStream.Name}' WHERE Id={fileStream.Id}";

            await sqlCommand.ExecuteScalarAsync(cancellationToken);

            // get filePath again
            sqlCommand.CommandText = $"SELECT Content.PathName() FROM {dbName}.dbo.UserFiles WHERE Id={fileStream.Id}";
            filePath = await sqlCommand.ExecuteScalarAsync(cancellationToken) as string;
        }

        await using SqlTransaction transaction = sqlConnection.BeginTransaction("mainTranaction");
        sqlCommand.Transaction = transaction;

        sqlCommand.CommandText = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()";
        var txContext = (byte[])await sqlCommand.ExecuteScalarAsync(cancellationToken);

        await using var sqlFileStream = new SqlFileStream(filePath!, txContext, FileAccess.Write);

        _logger.LogDebug("Writing to database");

        if (_useBufferingForBigFiles)
        {
            byte[] buffer = new byte[_bufferSizeForStreamCopy];
            int read, count = 0;
            while ((read = await fileStream.Content!.ReadAsync(buffer, cancellationToken)) > 0)
            {
                count++;
                await sqlFileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }

            _logger.LogDebug("IterationsCount:{count}", count);
        }
        else
        {
            await fileStream.Content!.CopyToAsync(sqlFileStream, cancellationToken);
        }

        sqlFileStream.Close();
        sqlCommand.Transaction.Commit();

        _logger.LogDebug("Updating record");

        // update file size
        sqlCommand.CommandText = $"UPDATE {dbName}.dbo.UserFiles SET Size={fileStream.Content!.Length}" +
            $" WHERE Id={fileStream.Id}";

        await sqlCommand.ExecuteScalarAsync(cancellationToken);

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
