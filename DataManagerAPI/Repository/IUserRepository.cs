using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Repository
{
    public interface IUserRepository
    {
        Task<ResultWrapper<User>> AddUser(User userToAdd);
        Task<UserCredentials?> GetUserCredentions(int userId);
    }
}
