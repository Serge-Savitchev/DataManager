using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using Microsoft.AspNetCore.WebUtilities;

namespace DataManagerAPI.Services;

public interface IUserDataService
{
    Task<ResultWrapper<UserDataDto>> AddUserData(AddUserDataDto userDataToAdd);
    Task<ResultWrapper<UserDataDto>> UpdateUserData(UserDataDto userDataToUpdate);
    Task<ResultWrapper<UserDataDto>> DeleteUserData(int userDataId);
    Task<ResultWrapper<UserDataDto>> GetUserData(int userDataId);
    Task<ResultWrapper<List<UserDataDto>>> GetUserDataByUserId(int userId);
    Task<bool> UploadFile(MultipartReader reader, MultipartSection? section);
}
