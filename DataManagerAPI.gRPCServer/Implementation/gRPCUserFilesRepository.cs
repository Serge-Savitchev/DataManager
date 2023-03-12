using DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.NLogger;
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
    private readonly ILogger<gRPCUserFilesRepository> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserFilesRepository"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public gRPCUserFilesRepository(IUserFilesRepository repository, ILogger<gRPCUserFilesRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> DeleteFileAsync(Int32Int32Request request, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.DeleteFileAsync(request.Value1, request.Value2,
             context.CancellationToken);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <summary>
    /// This method is implemented in <see cref="gRPCProtoService"/>
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
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetListAsync(request.Value, context.CancellationToken);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <summary>
    /// This method is implemented in <see cref="gRPCProtoService"/>
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
