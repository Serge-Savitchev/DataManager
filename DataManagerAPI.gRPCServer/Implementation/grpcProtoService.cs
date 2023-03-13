using DataManagerAPI.NLogger;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using static Grpc.ProtoService;

namespace DataManagerAPI.gRPCServer.Implementation;

/// <summary>
/// Service for streams processing. Implementaion of DownloadFileAsync and UploadFileAsync methods.
/// </summary>
public class gRPCProtoService : ProtoServiceBase
{
    private readonly IUserFilesRepository _repository;  // interface for working with database
    private readonly ILogger<gRPCProtoService> _logger;

    private readonly int _bufferSizeForStreamCopy = 1024 * 1024 * 4;    // default size of the buffer 4 MB

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserFilesRepository"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public gRPCProtoService(IUserFilesRepository repository, IConfiguration configuration, ILogger<gRPCProtoService> logger) : base()
    {
        _repository = repository;
        _logger = logger;

        if (int.TryParse(configuration["Buffering:Server:BufferSize"], out int size) && size > 0)
        {
            _bufferSizeForStreamCopy = size * 1024;
        }
        _logger = logger;
    }

    /// <summary>
    /// Uploads file via IUserFilesRepository
    /// </summary>
    /// <param name="requestStream"><see cref="Grpc.ResultWrapper"/></param>
    /// <param name="context"><see cref="ServerCallContext"/></param>
    /// <returns><see cref="Grpc.ResultWrapper"/></returns>
    public override async Task<Grpc.ResultWrapper> UploadFile(IAsyncStreamReader<Grpc.UserFileStream> requestStream,
        ServerCallContext context)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = new Grpc.ResultWrapper();
        string newFileName = string.Empty;

        try
        {
            Stream? outputStream = null;
            Grpc.UserFileStream? request = null;

            int count = 0;

            _logger.LogDebug("Copying file content to temporary file");

            // read content of the file and write it to temporary file
            await foreach (Grpc.UserFileStream message in requestStream.ReadAllAsync(context.CancellationToken))
            {
                count++;

                if (request == null)    // the first pass
                {
                    // open temporary file for writing
                    newFileName = Path.GetTempFileName();
                    outputStream = new System.IO.FileStream(newFileName, FileMode.Create);
                }

                request = message;   // request from gRPC client
                await outputStream!.WriteAsync(message.Content.Memory, context.CancellationToken);
            }

            _logger.LogDebug("IterationsCount:{count}", count);

            outputStream!.Close();  // copying finished

            // create request for repository
            var repositoryRequest = new Repository.Abstractions.Models.UserFileStream
            {
                Id = request!.UserFile.Id,
                UserDataId = request.UserFile.UserDataId,
                Name = request.UserFile.Name,
                Size = (long)request.UserFile.Size,
                BigFile = request.BigFile
            };

            // open stream from temporary file
            await using var inputStream = new System.IO.FileStream(newFileName, FileMode.Open, FileAccess.Read);
            await using var bufferedStream = new BufferedStream(inputStream, _bufferSizeForStreamCopy);
            repositoryRequest.Content = bufferedStream;

            _logger.LogDebug("Calling repository");
            // call repository
            ResultWrapper<Repository.Abstractions.Models.UserFile> repositoryResult =
                await _repository.UploadFileAsync(repositoryRequest, context.CancellationToken);

            // convert repository response to Grpc.ResultWrapper type
            result.Message = repositoryResult.Message ?? string.Empty;
            result.Success = repositoryResult.Success;
            result.StatusCode = repositoryResult.StatusCode;

            if (repositoryResult.Success && repositoryResult.Data != null)
            {
                result.Data = new Grpc.UserFileStream
                {
                    UserFile = new Grpc.UserFile
                    {
                        Id = repositoryResult.Data.Id,
                        UserDataId = repositoryResult.Data.UserDataId,
                        Name = repositoryResult.Data.Name,
                        Size = (ulong)repositoryResult.Data!.Size,
                    },
                    BigFile = request.BigFile
                };
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError(ex, "{@wrapper}", result);
        }
        finally
        {
            File.Delete(newFileName);   // delete temporary file
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <summary>
    /// Downloads file via IUserFilesRepository
    /// </summary>
    /// <param name="request"><see cref="Grpc.DownloadFileRequest"/></param>
    /// <param name="responseStream"><see cref="Grpc.ResultWrapper"/></param>
    /// <param name="context"><see cref="ServerCallContext"/></param>
    /// <returns></returns>
    public override async Task DownloadFile(Grpc.DownloadFileRequest request,
        IServerStreamWriter<Grpc.ResultWrapper> responseStream, ServerCallContext context)
    {
        // responseStream contains stream for writing content of file to be downloded

        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = new Grpc.ResultWrapper();

        _logger.LogDebug("Call repository");
        // call repository
        ResultWrapper<Repository.Abstractions.Models.UserFileStream> repositoryResult =
            await _repository.DownloadFileAsync(request.UserDataId, request.FileId, context.CancellationToken);

        _logger.LogDebug("Preparing response");

        // convert repository response to IServerStreamWriter<Grpc.ResultWrapper> type
        result.Success = repositoryResult.Success;
        result.Message = repositoryResult.Message ?? string.Empty;
        result.StatusCode = repositoryResult.StatusCode;

        try
        {
            if (repositoryResult.Success && repositoryResult.Data != null)
            {
                result.Data = new Grpc.UserFileStream
                {
                    UserFile = new Grpc.UserFile
                    {
                        Id = repositoryResult.Data.Id,
                        UserDataId = repositoryResult.Data.UserDataId,
                        Name = repositoryResult.Data.Name,
                        Size = (ulong)repositoryResult.Data.Size,
                    },
                    BigFile = repositoryResult.Data.BigFile
                };

                _logger.LogDebug("Copying file content to response stream using memory buffer");
                // copy file content to responseStream using memory buffer
                int read = 0;
                byte[] buffer = new byte[_bufferSizeForStreamCopy];

                int count = 0;
                while ((read = await repositoryResult.Data.Content!.ReadAsync(buffer, context.CancellationToken)) > 0)
                {
                    count++;
                    result.Data.Content = ByteString.CopyFrom(new ReadOnlySpan<byte>(buffer, 0, read));
                    await responseStream.WriteAsync(result, context.CancellationToken);
                }

                _logger.LogDebug("IterationsCount:{count}", count);
            }
            else
            {
                _logger.LogDebug("Copying file content directly tp response stream");
                await responseStream.WriteAsync(result, context.CancellationToken);
            }
        }
        catch (Exception ex)
        {
            result.Data = null;
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
            _logger.LogError(ex, "{@wrapper}", result);
        }

        _logger.LogInformation("Finished");
    }
}
