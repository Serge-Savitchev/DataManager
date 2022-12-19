using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;

namespace DataManagerAPI.Services;

public interface IUserService
{
    Task<ResultWrapper<UserDto>> AddUser(AddUserDto userToAdd);
}
