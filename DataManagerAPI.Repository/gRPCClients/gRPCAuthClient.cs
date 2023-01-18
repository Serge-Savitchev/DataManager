using DataManagerAPI.Repository.Abstractions.gRPCInterfaces;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Repository.gRPCClients;

public class gRPCAuthClient : IAuthRepository
{
    private readonly IgRPCAuthRepository _igRPCAuthRepository;

    public gRPCAuthClient(IgRPCAuthRepository igRPCAuthRepository)
    {
        _igRPCAuthRepository = igRPCAuthRepository;
    }

    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(int userId)
    {
        return _igRPCAuthRepository.GetUserDetailsByIdAsync(userId);
    }

    public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(string login)
    {
        return _igRPCAuthRepository.GetUserDetailsByLoginAsync(login);
    }

    public Task<ResultWrapper<int>> LoginAsync(string login, UserCredentials credentials)
    {
        return _igRPCAuthRepository.LoginAsync(login, credentials);
    }

    public Task<ResultWrapper<int>> RefreshTokenAsync(int userId, string? refreshToken)
    {
        return _igRPCAuthRepository.RefreshTokenAsync(userId, refreshToken);
    }

    public Task<ResultWrapper<User>> RegisterUserAsync(User userToAdd, UserCredentials userCredentials)
    {
        return _igRPCAuthRepository.RegisterUserAsync(userToAdd, userCredentials);
    }

    public Task<ResultWrapper<int>> UpdateUserPasswordAsync(int userId, UserCredentials credentials)
    {
        return _igRPCAuthRepository.UpdateUserPasswordAsync(userId, credentials);
    }

    public Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(int userId, RoleIds newRole)
    {
        return _igRPCAuthRepository.UpdateUserRoleAsync(userId, newRole);
    }
}
