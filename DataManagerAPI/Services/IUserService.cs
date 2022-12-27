using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Services;

public interface IUserService
{
    Task<ResultWrapper<UserDto>> DeleteUser(int userId);
    Task<ResultWrapper<UserDto>> GetUser(int userId);
    Task<ResultWrapper<List<UserDto>>> GetUsersByRole(string role);
    Task<ResultWrapper<List<UserDto>>> GetAllUsers();

}
