using DataManagerAPI.Repository.Models;
using DataManagerAPI.Shared.Helpers;

namespace DataManagerAPI.Repository.Interfaces;

public interface IUserRepository
{
    Task<ResultWrapper<User>> DeleteUser(int userId);
    Task<ResultWrapper<User>> GetUser(int userId);
    Task<ResultWrapper<List<User>>> GetUsersByRole(RoleIds roleId);
    Task<ResultWrapper<List<User>>> GetAllUsers();
    Task<ResultWrapper<int>> UpdateOwners(int ownerId, int[] users);
}

