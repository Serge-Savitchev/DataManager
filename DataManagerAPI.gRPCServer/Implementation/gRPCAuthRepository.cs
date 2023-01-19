using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

public class gRPCAuthRepository : IgRPCAuthRepository
{
    private readonly IAuthRepository _repository;

    public gRPCAuthRepository(IAuthRepository repository)
    {
        _repository = repository;
    }

    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(UserIdRequest request, CallContext context = default)
    {
        return _repository.GetUserDetailsByIdAsync(request.UserId);
    }

    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(LoginValueRequest login, CallContext context = default)
    {
        return _repository.GetUserDetailsByLoginAsync(login.Login);
    }

    public Task<ResultWrapper<int>> LoginAsync(LoginRequest request, CallContext context = default)
    {
        return _repository.LoginAsync(request.Login, request.Credentials!);
    }

    public Task<ResultWrapper<int>> RefreshTokenAsync(RefreshTokenRequest request, CallContext context = default)
    {
        return _repository.RefreshTokenAsync(request.UserId, request.RefreshToken);
    }

    public Task<ResultWrapper<User>> RegisterUserAsync(RegisterUserRequest request, CallContext context = default)
    {
        return _repository.RegisterUserAsync(request.User!, request.UserCredentials!);
    }

    public Task<ResultWrapper<int>> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CallContext context = default)
    {
        return _repository.UpdateUserPasswordAsync(request.UserId, request.UserCredentials!);
    }

    public Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(UpdateUserRoleRequest request, CallContext context = default)
    {
        return _repository.UpdateUserRoleAsync(request.UserId, request.Role);
    }
}
