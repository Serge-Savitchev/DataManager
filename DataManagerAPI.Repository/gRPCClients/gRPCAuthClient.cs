using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace DataManagerAPI.Repository.gRPCClients;

public class gRPCAuthClient : IAuthRepository
{
    private readonly IgRPCAuthRepository _igRPCAuthRepository;

    public gRPCAuthClient(GrpcChannel channel)
    {
        _igRPCAuthRepository = channel.CreateGrpcService<IgRPCAuthRepository>();
    }

    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(int userId)
    {
        return _igRPCAuthRepository.GetUserDetailsByIdAsync(new UserIdRequest { UserId = userId });
    }

    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(string login)
    {
        return _igRPCAuthRepository.GetUserDetailsByLoginAsync(new LoginValueRequest { Login = login });
    }

    public Task<ResultWrapper<int>> LoginAsync(string login, UserCredentials credentials)
    {
        return _igRPCAuthRepository.LoginAsync(new LoginRequest { Login = login, Credentials = credentials });
    }

    public Task<ResultWrapper<int>> RefreshTokenAsync(int userId, string? refreshToken)
    {
        return _igRPCAuthRepository.RefreshTokenAsync(new RefreshTokenRequest { UserId = userId, RefreshToken = refreshToken });
    }

    public Task<ResultWrapper<User>> RegisterUserAsync(User userToAdd, UserCredentials userCredentials)
    {
        return _igRPCAuthRepository.RegisterUserAsync(new RegisterUserRequest { User = userToAdd, UserCredentials = userCredentials });
    }

    public Task<ResultWrapper<int>> UpdateUserPasswordAsync(int userId, UserCredentials credentials)
    {
        return _igRPCAuthRepository.UpdateUserPasswordAsync(new UpdateUserPasswordRequest { UserId = userId, UserCredentials = credentials });
    }

    public Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(int userId, RoleIds newRole)
    {
        return _igRPCAuthRepository.UpdateUserRoleAsync(new UpdateUserRoleRequest { UserId = userId, Role = newRole });
    }
}
