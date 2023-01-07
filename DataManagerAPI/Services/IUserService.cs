using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;

namespace DataManagerAPI.Services;

public interface IUserService
{
    Task<ResultWrapper<UserDto>> DeleteUser(int userId);
    Task<ResultWrapper<UserDto>> GetUser(int userId);
    Task<ResultWrapper<List<UserDto>>> GetUsersByRole(string role);
    Task<ResultWrapper<List<UserDto>>> GetAllUsers();
    Task<ResultWrapper<int>> UpdateOwners(UpdateOwnerRequest request);
}
