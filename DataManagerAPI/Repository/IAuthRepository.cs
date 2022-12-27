using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Repository;

public interface IAuthRepository
{
    Task<ResultWrapper<User>> RegisterUser(User userToAdd, UserCredentials userCredentials);
    Task<ResultWrapper<int>> Login(string login, UserCredentials credentials);
    Task<ResultWrapper<int>> RefreshToken(int userId, string? token);
    Task<ResultWrapper<UserCredentials>> GetUserCredentials(int userId);
    Task<ResultWrapper<int>> UpdateUserPassword(int userId, UserCredentials credentials);
    Task<ResultWrapper<UserCredentialsData>> GetUserByLogin(string login);
}
