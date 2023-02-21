using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Grpc.Core;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace DataManagerAPI.Repository.gRPCClients;

/// <summary>
/// Implementation of <see cref="IUserFilesRepository"/> for gRPC client.
/// </summary>
public class gRPCUserFilesClient : IUserFilesRepository
{
    private readonly IgRPCUserFilesRepository _igRPCUserFilesRepository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="channel"><see cref="GrpcChannel"/></param>
    public gRPCUserFilesClient(GrpcChannel channel)
    {
        _igRPCUserFilesRepository = channel.CreateGrpcService<IgRPCUserFilesRepository>();
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> DeleteFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserFilesRepository.DeleteFileAsync(new Int32Int32Request { Value1 = userDataId, Value2 = fileId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserFileStream>> DownloadFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserFilesRepository.DownloadFileAsync(new Int32Int32Request { Value1 = userDataId, Value2 = fileId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserFile[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserFilesRepository.GetListAsync(new Int32Request { Value = userDataId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserFile>> UploadFileAsync(UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        return _igRPCUserFilesRepository.UploadFileAsync(fileStream,
            new CallOptions(cancellationToken: cancellationToken));
    }
}
