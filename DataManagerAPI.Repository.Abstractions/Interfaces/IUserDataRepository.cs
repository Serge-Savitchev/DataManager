using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

[ServiceContract]
public interface IUserDataRepository
{
    [OperationContract]
    Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd);

    [OperationContract]
    Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate);

    [OperationContract]
    Task<ResultWrapper<UserData>> DeleteUserDataAsync(int userDataId);

    [OperationContract]
    Task<ResultWrapper<UserData>> GetUserDataAsync(int userDataId);

    [OperationContract]
    Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(int userId);
}
