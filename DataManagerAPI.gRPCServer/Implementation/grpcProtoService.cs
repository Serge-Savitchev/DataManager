using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using static Grpc.ProtoService;

namespace DataManagerAPI.gRPCServer.Implementation;

/// <summary>
/// Service for streams processing. Implementaion of DownloadFileAsync and UploadFileAsync methods.
/// </summary>
public class grpcProtoService : ProtoServiceBase
{
    private readonly IUserFilesRepository _repository;  // interface for working with database
    private readonly int _bufferSizeForStreamCopy = 1024 * 1024 * 4;    // default size of the buffer 4 MB

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserFilesRepository"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    public grpcProtoService(IUserFilesRepository repository, IConfiguration configuration) : base()
    {
        _repository = repository;
        if (int.TryParse(configuration["Buffering:Server:BufferSize"], out int size) && size > 0)
        {
            _bufferSizeForStreamCopy = size * 1024;
        }
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
        var result = new Grpc.ResultWrapper();
        string newFileName = string.Empty;  // Path.Combine(Path.GetTempPath(), "DataManager\\UploadFile.tmp");

        try
        {
            Stream? outputStream = null;
            Grpc.UserFileStream? request = null;

            int count = 0;

            // read content of the file and write it to temporary file
            await foreach (Grpc.UserFileStream message in requestStream.ReadAllAsync(context.CancellationToken))
            {
                if (request == null)    // the first pass
                {
                    // open temporary file for writing
                    newFileName = Path.GetTempFileName();
                    outputStream = new System.IO.FileStream(newFileName, FileMode.Create);
                }

                request = message;   // request from gRPC client

                await outputStream!.WriteAsync(message.Content.Memory, context.CancellationToken);

                Console.WriteLine($"Upload: {++count}\t{message.Content.Length}");
            }

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
        }
        finally
        {
            File.Delete(newFileName);   // delete temporary file
        }

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

        var result = new Grpc.ResultWrapper();

        // call repository
        ResultWrapper<Repository.Abstractions.Models.UserFileStream> repositoryResult =
            await _repository.DownloadFileAsync(request.UserDataId, request.FileId, context.CancellationToken);

        // convert repository response to IServerStreamWriter<Grpc.ResultWrapper> type
        result.Success = repositoryResult.Success;
        result.Message = repositoryResult.Message ?? string.Empty;
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
                    Size = (ulong)repositoryResult.Data.Size,
                },
                BigFile = repositoryResult.Data.BigFile
            };

            // copy file content to responseStream using memory buffer
            int read = 0;
            byte[] buffer = new byte[_bufferSizeForStreamCopy];

            int count = 0;
            while ((read = await repositoryResult.Data.Content!.ReadAsync(buffer, context.CancellationToken)) > 0)
            {
                result.Data.Content = ByteString.CopyFrom(new ReadOnlySpan<byte>(buffer, 0, read));
                await responseStream.WriteAsync(result, context.CancellationToken);

                Console.WriteLine($"Download: {++count}\t{read}");
            }
        }
    }
}
