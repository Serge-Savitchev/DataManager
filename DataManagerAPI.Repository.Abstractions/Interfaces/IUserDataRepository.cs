using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

[ServiceContract]
public interface IUserDataRepository
{
    [OperationContract]
    Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd);

    [OperationContract]
    Task<ResultWrapper<UserData>> UpdateUserData(UserData userDataToUpdate);

    [OperationContract]
    Task<ResultWrapper<UserData>> DeleteUserData(int userDataId);

    [OperationContract]
    Task<ResultWrapper<UserData>> GetUserData(int userDataId);

    [OperationContract]
    Task<ResultWrapper<List<UserData>>> GetUserDataByUserId(int userId);
}
