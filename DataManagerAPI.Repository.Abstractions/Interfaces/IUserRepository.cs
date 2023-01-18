using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

[ServiceContract]
public interface IUserRepository
{
    [OperationContract]
    Task<ResultWrapper<User>> DeleteUser(int userId);

    [OperationContract]
    Task<ResultWrapper<User>> GetUser(int userId);

    [OperationContract]
    Task<ResultWrapper<List<User>>> GetUsersByRole(RoleIds roleId);

    [OperationContract]
    Task<ResultWrapper<List<User>>> GetAllUsers();

    [OperationContract]
    Task<ResultWrapper<int>> UpdateOwners(int ownerId, int[] users);
}

