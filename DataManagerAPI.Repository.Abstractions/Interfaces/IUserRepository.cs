using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

public interface IUserRepository
{
    Task<ResultWrapper<User>> DeleteUser(int userId);
    Task<ResultWrapper<User>> GetUser(int userId);
    Task<ResultWrapper<List<User>>> GetUsersByRole(RoleIds roleId);
    Task<ResultWrapper<List<User>>> GetAllUsers();
    Task<ResultWrapper<int>> UpdateOwners(int ownerId, int[] users);
}

