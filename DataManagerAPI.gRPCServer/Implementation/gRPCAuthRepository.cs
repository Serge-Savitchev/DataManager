using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
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

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IAuthRepository"/></param>
    public gRPCAuthRepository(IAuthRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(Int32Request request, CallContext context = default)
    {
        return _repository.GetUserDetailsByIdAsync(request.Value);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(StringRequest login, CallContext context = default)
    {
        return _repository.GetUserDetailsByLoginAsync(login.Value);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> LoginAsync(LoginRequest request, CallContext context = default)
    {
        return _repository.LoginAsync(request.Login, request.Credentials!);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> RefreshTokenAsync(RefreshTokenRequest request, CallContext context = default)
    {
        return _repository.RefreshTokenAsync(request.UserId, request.RefreshToken);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> RegisterUserAsync(RegisterUserRequest request, CallContext context = default)
    {
        return _repository.RegisterUserAsync(request.User!, request.UserCredentials!);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CallContext context = default)
    {
        return _repository.UpdateUserPasswordAsync(request.UserId, request.UserCredentials!);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(UpdateUserRoleRequest request, CallContext context = default)
    {
        return _repository.UpdateUserRoleAsync(request.UserId, request.Role);
    }
}
