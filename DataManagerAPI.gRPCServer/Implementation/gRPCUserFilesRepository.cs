using Azure.Core;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

/// <summary>
/// Implementation of <see cref="IgRPCUserFilesRepository"/> for gRPC server,
/// excepting DownloadFileAsync and UploadFileAsync methods.
/// </summary>
public class gRPCUserFilesRepository : IgRPCUserFilesRepository
{
    private readonly IUserFilesRepository _repository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserFilesRepository"/></param>
    public gRPCUserFilesRepository(IUserFilesRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> DeleteFileAsync(Int32Int32Request request, CallContext context = default)
    {
        return _repository.DeleteFileAsync(request.Value1, request.Value2,
             context.CancellationToken);
    }

    /// <summary>
    /// This method is implemented in <see cref="grpcProtoService"/>
    /// </summary>
    /// <param name="request">Not used</param>
    /// <param name="context">Not used</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ResultWrapper<UserFileStream>> DownloadFileAsync(Int32Int32Request request, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserFile[]>> GetListAsync(Int32Request request, CallContext context = default)
    {
        return _repository.GetListAsync(request.Value, context.CancellationToken);
    }

    /// <summary>
    /// This method is implemented in <see cref="grpcProtoService"/>
    /// </summary>
    /// <param name="fileStream">Not used</param>
    /// <param name="context">Not used</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<ResultWrapper<UserFile>> UploadFileAsync(UserFileStream fileStream, CallContext context = default)
    {
        throw new NotImplementedException();
    }
}
