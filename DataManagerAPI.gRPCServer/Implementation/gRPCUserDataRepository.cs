using DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.NLogger;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

/// <summary>
/// Implementation of <see cref="IgRPCUserDataRepository"/> for gRPC server.
/// </summary>
public class gRPCUserDataRepository : IgRPCUserDataRepository
{
    private readonly IUserDataRepository _repository;
    private readonly ILogger<gRPCUserDataRepository> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserDataRepository"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public gRPCUserDataRepository(IUserDataRepository repository, ILogger<gRPCUserDataRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.AddUserDataAsync(userDataToAdd);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> DeleteUserDataAsync(Int32Int32Request userDataId, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.DeleteUserDataAsync(userDataId.Value1, userDataId.Value2);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> GetUserDataAsync(Int32Int32Request userDataId, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetUserDataAsync(userDataId.Value1, userDataId.Value2);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(Int32Request userId, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetUserDataByUserIdAsync(userId.Value);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.UpdateUserDataAsync(userDataToUpdate);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> GetUserAsync(Int32Request userDataId, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetUserAsync(userDataId.Value);

        _logger.LogInformation("Finished");

        return result;
    }

}
