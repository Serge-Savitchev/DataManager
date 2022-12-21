using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Repository;

public interface IUserDataRepository
{
    Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd);
    Task<ResultWrapper<UserData>> UpdateUserData(UserData userDataToUpdate);
    Task<ResultWrapper<UserData>> DeleteUserData(int userDataId);
    Task<ResultWrapper<UserData>> GetUserData(int userDataId);
    Task<ResultWrapper<List<UserData>>> GetUserDataByUserId(int userId);
}
