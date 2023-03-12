using DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.NLogger;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

/// <summary>
/// Implementation of <see cref="IgRPCUsersRepository"/> for gRPC server.
/// </summary>
public class gRPCUsersRepository : IgRPCUsersRepository
{
    private readonly IUsersRepository _repository;
    private readonly ILogger<gRPCUsersRepository> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUsersRepository"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public gRPCUsersRepository(IUsersRepository repository, ILogger<gRPCUsersRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> DeleteUserAsync(Int32Request userId, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.DeleteUserAsync(userId.Value);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User[]>> GetAllUsersAsync(StringRequest empty, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetAllUsersAsync();

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> GetUserAsync(Int32Request userId, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetUserAsync(userId.Value);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleRequest roleId, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetUsersByRoleAsync(roleId.RoleId);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> UpdateOwnersAsync(UpdateOwnersRequest request, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.UpdateOwnersAsync(request.OwnerId, request.Users);

        _logger.LogInformation("Finished");

        return result;
    }
}
