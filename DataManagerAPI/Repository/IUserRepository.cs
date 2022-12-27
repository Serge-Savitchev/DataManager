using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Repository
{
    public interface IUserRepository
    {
        Task<ResultWrapper<User>> DeleteUser(int userId);
        Task<ResultWrapper<User>> GetUser(int userId);
        Task<ResultWrapper<List<User>>> GetUsersByRole(RoleIds roleId);
        Task<ResultWrapper<List<User>>> GetAllUsers();
    }
}
