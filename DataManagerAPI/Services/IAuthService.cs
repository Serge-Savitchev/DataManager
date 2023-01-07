using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;

namespace DataManagerAPI.Services;

public interface IAuthService
{
    Task<ResultWrapper<UserDto>> RegisterUser(RegisterUserDto userToAdd);
    Task<ResultWrapper<LoginUserResponseDto>> Login(LoginUserDto loginData);
    void LogOut(int userId);
    Task<ResultWrapper<TokenApiModelDto>> RefreshToken(TokenApiModelDto tokenData);
    Task<ResultWrapper<int>> Revoke(int userId);
    Task<ResultWrapper<int>> UpdateUserPassword(int userId, string newPassword);
    Task<ResultWrapper<string>> UpdateUserRole(int userId, string newRole);
    Task<ResultWrapper<UserDetailsDto>> GetUserDetails(int userId);
}
