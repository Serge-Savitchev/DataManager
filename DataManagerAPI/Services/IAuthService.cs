﻿using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

namespace DataManagerAPI.Services;

public interface IAuthService
{
    Task<ResultWrapper<UserDto>> RegisterUser(RegisterUserDto userToAdd);
    Task<ResultWrapper<LoginUserResponseDto>> Login(LoginUserDto loginData);
    Task<ResultWrapper<TokenApiModelDto>> RefreshToken(TokenApiModelDto tokenData);
    Task<ResultWrapper<int>> Revoke(int userId);
    Task<ResultWrapper<int>> UpdateUserPassword(int userId, string newPassword);
    Task<ResultWrapper<string>> UpdateUserRole(int userId, string newRole);
    Task<ResultWrapper<UserDetailsDto>> GetUserDetails(int userId);
}
