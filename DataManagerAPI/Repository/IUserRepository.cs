using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Repository
{
    public interface IUserRepository
    {
        Task<ResultWrapper<User>> AddUser(User userToAdd);
        Task<ResultWrapper<UserCredentials>> GetUserCredentials(int userId);
        Task<ResultWrapper<User>> UpdateUserCredentials(int userId, UserCredentials credentials);
        Task<ResultWrapper<User>> UpdateUser(User userToUpdate);
        Task<ResultWrapper<User>> DeleteUser(int userId);
        Task<ResultWrapper<User>> GetUser(int userId);
        Task<ResultWrapper<List<User>>> GetUsersByRole(RoleId roleId);
        Task<ResultWrapper<List<User>>> GetAllUsers();
    }
}
