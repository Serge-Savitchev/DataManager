using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

[ServiceContract]
public interface IUserRepository
{
    [OperationContract]
    Task<ResultWrapper<User>> DeleteUserAsync(int userId);

    [OperationContract]
    Task<ResultWrapper<User>> GetUserAsync(int userId);

    [OperationContract]
    Task<ResultWrapper<List<User>>> GetUsersByRoleAsync(RoleIds roleId);

    [OperationContract]
    Task<ResultWrapper<List<User>>> GetAllUsersAsync();

    [OperationContract]
    Task<ResultWrapper<int>> UpdateOwnersAsync(int ownerId, int[] users);
}

