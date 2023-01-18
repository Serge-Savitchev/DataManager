using DataManagerAPI.Repository.Abstractions.gRPCInterfaces;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.gRPCServer.Implementation
{
    public class gRPCAuthRepository : IgRPCAuthRepository
    {
        private readonly IAuthRepository _repository;

        public gRPCAuthRepository(IAuthRepository repository)
        {
            _repository = repository;
        }

        public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(int userId)
        {
            return _repository.GetUserDetailsByIdAsync(userId);
        }

        public Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(string login)
        {
            return _repository.GetUserDetailsByLoginAsync(login);
        }

        public Task<ResultWrapper<int>> LoginAsync(string login, UserCredentials credentials)
        {
            return _repository.LoginAsync(login, credentials);
        }

        public Task<ResultWrapper<int>> RefreshTokenAsync(int userId, string? refreshToken)
        {
            return _repository.RefreshTokenAsync(userId, refreshToken);
        }

        public Task<ResultWrapper<User>> RegisterUserAsync(User userToAdd, UserCredentials userCredentials)
        {
            return _repository.RegisterUserAsync(userToAdd, userCredentials);
        }

        public Task<ResultWrapper<int>> UpdateUserPasswordAsync(int userId, UserCredentials credentials)
        {
            return _repository.UpdateUserPasswordAsync(userId, credentials);
        }

        public Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(int userId, RoleIds newRole)
        {
            return _repository.UpdateUserRoleAsync(userId, newRole);
        }
    }
}
