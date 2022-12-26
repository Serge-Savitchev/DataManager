using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Repository
{
    public interface IUserRepository
    {
        Task<ResultWrapper<User>> RegisterUser(User userToAdd, UserCredentials userCredentials);
        Task<ResultWrapper<int>> Login(string login, UserCredentials credentials);
        Task<ResultWrapper<UserCredentials>> GetUserCredentials(int userId);
        Task<ResultWrapper<string>> UpdateUserPassword(int userId, UserCredentials credentials);
        //Task<ResultWrapper<User>> UpdateUser(User userToUpdate);
        Task<ResultWrapper<User>> DeleteUser(int userId);
        Task<ResultWrapper<User>> GetUser(int userId);
        Task<ResultWrapper<List<User>>> GetUsersByRole(RoleId roleId);
        Task<ResultWrapper<UserCredentialsData>> GetUserByLogin(string login);
        Task<ResultWrapper<List<User>>> GetAllUsers();
    }
}
