using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;

namespace DataManagerAPI.Services;

public interface IUserDataService
{
    Task<ResultWrapper<UserDataDto>> AddUserData(AddUserDataDto userDataToAdd);
}
