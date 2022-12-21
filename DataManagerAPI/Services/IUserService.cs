using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Services;

public interface IUserService
{
    Task<ResultWrapper<UserDto>> AddUser(AddUserDto userToAdd);
    Task<ResultWrapper<string>> UpdateUserCredentials(int userId, string newLogin, string newPassword);
    Task<ResultWrapper<string>> GetUserCredentials(int userId);
    Task<ResultWrapper<UserDto>> UpdateUser(UserDto userToUpdate);
    Task<ResultWrapper<UserDto>> DeleteUser(int userId);
    Task<ResultWrapper<UserDto>> GetUser(int userId);
    Task<ResultWrapper<List<UserDto>>> GetUsersByRole(string role);
    Task<ResultWrapper<List<UserDto>>> GetAllUsers();

}
