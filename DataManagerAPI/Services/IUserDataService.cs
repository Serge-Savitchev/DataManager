using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;

namespace DataManagerAPI.Services;

public interface IUserDataService
{
    Task<ResultWrapper<UserDataDto>> AddUserData(AddUserDataDto userDataToAdd);
    Task<ResultWrapper<UserDataDto>> UpdateUserData(UserDataDto userDataToUpdate);
    Task<ResultWrapper<UserDataDto>> DeleteUserData(int userDataId);
    Task<ResultWrapper<UserDataDto>> GetUserData(int userDataId);
    Task<ResultWrapper<List<UserDataDto>>> GetUserDataByUserId(int userId);
}
