using DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.NLogger;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

/// <summary>
/// Implementation of <see cref="IgRPCAuthRepository"/> for gRPC server.
/// </summary>
public class gRPCAuthRepository : IgRPCAuthRepository
{
    private readonly IAuthRepository _repository;
    private readonly ILogger<gRPCAuthRepository> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IAuthRepository"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public gRPCAuthRepository(IAuthRepository repository, ILogger<gRPCAuthRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(Int32Request request, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetUserDetailsByIdAsync(request.Value);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(StringRequest login, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.GetUserDetailsByLoginAsync(login.Value);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> LoginAsync(LoginRequest request, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.LoginAsync(request.Login, request.Credentials!);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> RefreshTokenAsync(RefreshTokenRequest request, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.RefreshTokenAsync(request.UserId, request.RefreshToken);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> RegisterUserAsync(RegisterUserRequest request, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.RegisterUserAsync(request.User!, request.UserCredentials!);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.UpdateUserPasswordAsync(request.UserId, request.UserCredentials!);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(UpdateUserRoleRequest request, CallContext context = default)
    {
        using var scope = _logger
            .BeginScope(new[] { new KeyValuePair<string, object>(NLoggerConstants.ActivityIdKey, gRPCServerHelper.GetRemoteActivityTraceId(context)) });

        _logger.LogInformation("Started");

        var result = _repository.UpdateUserRoleAsync(request.UserId, request.Role);

        _logger.LogInformation("Finished");

        return result;
    }
}
