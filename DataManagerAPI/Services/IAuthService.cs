using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;

namespace DataManagerAPI.Services;

public interface IAuthService
{
    Task<ResultWrapper<UserDto>> RegisterUser(RegisterUserDto userToAdd);
    Task<ResultWrapper<LoginUserResponseDto>> Login(LoginUserDto loginData);
    Task<ResultWrapper<TokenApiModelDto>> RefreshToken(TokenApiModelDto tokenData);
    Task<ResultWrapper<int>> Revoke(int userId);
    Task<ResultWrapper<int>> UpdateUserPassword(int userId, string newPassword);
}
