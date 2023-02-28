using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using ProtoBuf.Grpc.Client;

namespace DataManagerAPI.Repository.gRPCClients;

/// <summary>
/// Implementation of <see cref="IUserFilesRepository"/> for gRPC client.
/// </summary>
public class gRPCUserFilesClient : IUserFilesRepository
{
    private readonly IgRPCUserFilesRepository _igRPCUserFilesRepository;
    private readonly GrpcClient.ProtoService.ProtoServiceClient _protoClient;
    private readonly int _bufferSizeForStreamCopy = 1024 * 1024 * 4;    // default size of the buffer 4 MB

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="channel"><see cref="GrpcChannel"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    public gRPCUserFilesClient(GrpcChannel channel, IConfiguration configuration)
    {
        _igRPCUserFilesRepository = channel.CreateGrpcService<IgRPCUserFilesRepository>();
        _protoClient = new GrpcClient.ProtoService.ProtoServiceClient(channel);

        if (int.TryParse(configuration["Buffering:Server:BufferSize"], out int size) && size > 0)
        {
            _bufferSizeForStreamCopy = size * 1024;
        }
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> DeleteFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserFilesRepository.DeleteFileAsync(new Int32Int32Request { Value1 = userDataId, Value2 = fileId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<Abstractions.Models.UserFile[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserFilesRepository.GetListAsync(new Int32Request { Value = userDataId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<Abstractions.Models.UserFile>> UploadFileAsync(
        Abstractions.Models.UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<Abstractions.Models.UserFile>
        {
            Success = true
        };

        try
        {
            // create request for gRPC server
            var request = new GrpcClient.UserFileStream
            {
                UserFile = new GrpcClient.UserFile
                {
                    Id = fileStream.Id,
                    UserDataId = fileStream.UserDataId,
                    Name = fileStream.Name,
                    Size = (ulong)fileStream.Size,
                },
                BigFile = fileStream.BigFile
            };

            CallOptions context = new(cancellationToken: cancellationToken);

            // call gRPC service
            using var call = _protoClient.UploadFile(context);

            // now call.RequestStream contains stream for writing file content to gRPC service.

            int read = 0;

            // copy file content to gRPC service using memory buffer
            byte[] buffer = new byte[_bufferSizeForStreamCopy];

            while ((read = await fileStream.Content!.ReadAsync(buffer, cancellationToken)) > 0)
            {
                request.Content = ByteString.CopyFrom(new ReadOnlySpan<byte>(buffer, 0, read));
                await call.RequestStream.WriteAsync(request, context.CancellationToken);
            }

            await call.RequestStream.CompleteAsync();   // copying finished

            GrpcClient.ResultWrapper gRPCresponse = await call; // get gRPC service response

            // convert gRPC service response to ResultWrapper<Abstractions.Models.UserFile> type
            result = new ResultWrapper<Abstractions.Models.UserFile>
            {
                Data = gRPCresponse.Success ?
                    new Abstractions.Models.UserFile
                    {
                        UserDataId = gRPCresponse.Data.UserFile.UserDataId,
                        Id = gRPCresponse.Data.UserFile.Id,
                        Name = gRPCresponse.Data.UserFile.Name,
                        Size = (long)gRPCresponse.Data.UserFile.Size
                    } : null,
                Success = gRPCresponse.Success,
                Message = string.IsNullOrWhiteSpace(gRPCresponse.Message) ? null : gRPCresponse.Message,
                StatusCode = gRPCresponse.StatusCode
            };
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = ResultStatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<Abstractions.Models.UserFileStream>> DownloadFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<Abstractions.Models.UserFileStream>();

        try
        {
            // create request for gRPC service
            var request = new GrpcClient.DownloadFileRequest
            {
                UserDataId = userDataId,
                FileId = fileId
            };

            string newFileName = string.Empty; // Path.Combine(Path.GetTempPath(), "DataManager\\DowloadFile.tmp");

            CallOptions context = new(cancellationToken: cancellationToken);
            FileStream? inputStream = null;

            // call gRPC service
            using var call = _protoClient.DownloadFile(request, context);

            // now call.ResponseStream contains content of file to be downloaded

            // copy content of call.ResponseStream to temporary file

            GrpcClient.ResultWrapper? gRPCresponse = null;
            await foreach (var responseLocal in call.ResponseStream.ReadAllAsync(context.CancellationToken))
            {
                if (gRPCresponse == null)   // the first pass
                {
                    // open file for writing
                    newFileName = Path.GetTempFileName();
                    inputStream = new FileStream(newFileName, FileMode.Create);
                }
                gRPCresponse = responseLocal;   // response from gRPC service

                if (!gRPCresponse.Success)
                {
                    break;
                }

                await inputStream!.WriteAsync(responseLocal.Data.Content.Memory, context.CancellationToken);
            }

            inputStream!.Close();   // copying finished

            // open out stream
            var outputStream = gRPCresponse!.Success ?
                new FileStream(newFileName, FileMode.Open, FileAccess.Read,
                    FileShare.None, _bufferSizeForStreamCopy, FileOptions.SequentialScan | FileOptions.DeleteOnClose) : null;

            // convert response from gRPC service to ResultWrapper<Abstractions.Models.UserFileStream>
            result = new ResultWrapper<Abstractions.Models.UserFileStream>
            {
                Data = gRPCresponse!.Success ?
                    new Abstractions.Models.UserFileStream
                    {
                        UserDataId = gRPCresponse.Data.UserFile.UserDataId,
                        Id = gRPCresponse.Data.UserFile.Id,
                        Name = gRPCresponse.Data.UserFile.Name,
                        Size = (long)gRPCresponse.Data.UserFile.Size,
                        Content = new BufferedStream(outputStream, _bufferSizeForStreamCopy)  // content of file to be downloaded
                    } : null,
                Success = gRPCresponse.Success && gRPCresponse.Data != null,
                Message = string.IsNullOrWhiteSpace(gRPCresponse.Message) ? null : gRPCresponse.Message,
                StatusCode = gRPCresponse.StatusCode
            };
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.StatusCode = ResultStatusCodes.Status500InternalServerError;
        }

        return result;
    }
}
