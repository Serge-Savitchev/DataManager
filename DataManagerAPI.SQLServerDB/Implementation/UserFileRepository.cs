using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataManagerAPI.SQLServerDB.Implementation;

/// <summary>
/// Implementation of <see cref="IUserFileRepository"/>.
/// </summary>
public class UserFileRepository : IUserFileRepository
{
    private readonly UsersDBContext _context;
    private readonly bool _useBufferingForBigFiles;
    private readonly bool _useBufferingForSmallFiles;

    private readonly int _bufferSizeForStreamCopy = 1024 * 1024 * 4;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context">Database context. <see cref="UsersDBContext"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    public UserFileRepository(UsersDBContext context, IConfiguration configuration)
    {
        _context = context;

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

        var record = await _context.UserFiles.FirstOrDefaultAsync(x => x.Id == fileId && x.UserDataId == userDataId, cancellationToken);
        if (record != null)
        {
            _context.UserFiles.Remove(record);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileStream>> DownloadFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserFileStream>();

        var sqlConnection = (_context.Database.GetDbConnection() as SqlConnection)!;
        var dbName = MigrationExtensions.ExtructDBNameFromConnectionString(_context.Database.GetConnectionString()!);

        sqlConnection.Open();

        var sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;

        long? contentSize = null, dataSize = null, fileSize = null;
        string? fileName = null;

        var request = $"SELECT LEN(Content), LEN(Data), Size, Name FROM {dbName}.dbo.UserFiles" +
            $" WHERE Id={fileId} AND UserDataId={userDataId}";
        sqlCommand.CommandText = request;
        using (SqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken))
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
            result.StatusCode = StatusCodes.Status404NotFound;
            return result;
        }

        try
        {

            Stream? outStream = null;

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
                using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    dataSize = reader.GetBytes(0, 0, data, 0, (int)dataSize);
                    outStream = new MemoryStream(data);
                }
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
                Content = outStream
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

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFile[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserFile[]>
        {
            Success = true
        };

        var userData = await FindUserData<UserFile[]>(userDataId, cancellationToken);
        if (!userData.Success)
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
        if (!userData.Success)     // there is no UserData
        {
            return userData;
        }

        var result = new ResultWrapper<UserFile>
        {
            Success = true
        };


        try
        {
            var sqlConnection = (_context.Database.GetDbConnection() as SqlConnection)!;
            var dbName = MigrationExtensions.ExtructDBNameFromConnectionString(_context.Database.GetConnectionString()!);

            sqlConnection.Open();

            var sqlCommand = new SqlCommand();

            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandText = $"SELECT Name FROM {dbName}.dbo.UserFiles WHERE Id={fileStream.Id} AND UserDataId={fileStream.UserDataId}";

            var fileName = await sqlCommand.ExecuteScalarAsync(cancellationToken) as string;

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
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    private async Task ProcessFile(string? fileName, string dbName, SqlConnection sqlConnection,
        UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        var sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandTimeout = 0;

        MemoryStream memoryStream = new MemoryStream();

        if (_useBufferingForSmallFiles)
        {
            byte[] buffer = new byte[_bufferSizeForStreamCopy];
            int read, count = 0;
            while ((read = await fileStream.Content!.ReadAsync(buffer, cancellationToken)) > 0)
            {
                Console.WriteLine($"{++count}\t{read}");
                await memoryStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
        }
        else
        {
            await fileStream.Content!.CopyToAsync(memoryStream, cancellationToken);
        }

        string request = string.Empty;

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
    }

    private async Task ProcessBigFile(string? fileName, string dbName, SqlConnection sqlConnection,
        UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        var sqlCommand = new SqlCommand();
        sqlCommand.Connection = sqlConnection;
        sqlCommand.CommandTimeout = 0;

        sqlCommand.CommandText = $"SELECT Content.PathName() FROM {dbName}.dbo.UserFiles WHERE Id={fileStream.Id}";

        string? filePath = await sqlCommand.ExecuteScalarAsync(cancellationToken) as string;

        if (fileName == null)   // new file
        {
            var request = $"INSERT INTO {dbName}.dbo.UserFiles (FileId, UserDataId, Name, Size, Content, Data)" +
            " OUTPUT INSERTED.[Id]" +
            $" VALUES(NEWID(), {fileStream.UserDataId}, '{fileStream.Name}', 0, 0, NULL)";
            sqlCommand.CommandText = request;
            fileStream.Id = (await sqlCommand.ExecuteScalarAsync(cancellationToken) as int?)!.Value;
        }

        if (filePath == null)  // record exists, but FILESTREAM data is empty
        {
            // create empty data
            sqlCommand.CommandText = $"UPDATE {dbName}.dbo.UserFiles SET Size=0, Content=0, Data=NULL," +
                $" Name='{fileStream.Name}' WHERE Id={fileStream.Id}";

            await sqlCommand.ExecuteScalarAsync(cancellationToken);

            // get filePath again
            sqlCommand.CommandText = $"SELECT Content.PathName() FROM {dbName}.dbo.UserFiles WHERE Id={fileStream.Id}";
            filePath = await sqlCommand.ExecuteScalarAsync(cancellationToken) as string;
        }

        SqlTransaction transaction = sqlConnection.BeginTransaction("mainTranaction");
        sqlCommand.Transaction = transaction;

        sqlCommand.CommandText = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()";
        var txContext = (byte[])await sqlCommand.ExecuteScalarAsync(cancellationToken);

        var sqlFileStream = new SqlFileStream(filePath!, txContext, FileAccess.Write);

        if (_useBufferingForBigFiles)
        {
            byte[] buffer = new byte[_bufferSizeForStreamCopy];
            int read, count = 0;
            while ((read = await fileStream.Content!.ReadAsync(buffer, cancellationToken)) > 0)
            {
                Console.WriteLine($"{++count}\t{read}");
                await sqlFileStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }
        }
        else
        {
            await fileStream.Content!.CopyToAsync(sqlFileStream, cancellationToken);
        }

        sqlFileStream.Close();
        sqlCommand.Transaction.Commit();

        // update file size
        sqlCommand.CommandText = $"UPDATE {dbName}.dbo.UserFiles SET Size={fileStream.Content!.Length}" +
            $" WHERE Id={fileStream.Id}";

        await sqlCommand.ExecuteScalarAsync(cancellationToken);
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
