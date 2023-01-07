using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

public interface IUserDataRepository
{
    Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd);
    Task<ResultWrapper<UserData>> UpdateUserData(UserData userDataToUpdate);
    Task<ResultWrapper<UserData>> DeleteUserData(int userDataId);
    Task<ResultWrapper<UserData>> GetUserData(int userDataId);
    Task<ResultWrapper<List<UserData>>> GetUserDataByUserId(int userId);
}
