﻿namespace DataManagerAPI.Dto.Interfaces;

/// <summary>
/// This interface provides users management.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register new user.
    /// </summary>
    /// <param name="userToAdd"><see cref="RegisteredUserDto"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>New user. <see cref="UserDto"/></returns>
    Task<ResultWrapperDto<UserDto>> RegisterUser(RegisteredUserDto userToAdd, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates and authorizes user.
    /// </summary>
    /// <param name="loginData"><see cref="LoginUserDto"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Pair of token. <see cref="LoginUserResponseDto"/></returns>
    Task<ResultWrapperDto<LoginUserResponseDto>> Login(LoginUserDto loginData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout user.
    /// </summary>
    /// <param name="userId">User Id</param>
    void LogOut(int userId);

    /// <summary>
    /// Refreshes user's tokens.
    /// </summary>
    /// <param name="tokenData"><see cref="TokenApiModelDto"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Pair of new tokens. <see cref="TokenApiModelDto"/></returns>
    Task<ResultWrapperDto<TokenApiModelDto>> RefreshToken(TokenApiModelDto tokenData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes user's tokens.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>User Id</returns>
    Task<ResultWrapperDto<int>> Revoke(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user's password.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="newPassword">New password. <see cref="string"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>User Id</returns>
    Task<ResultWrapperDto<int>> UpdateUserPassword(int userId, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update user's role.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="newRole">New role name</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>New role name</returns>
    Task<ResultWrapperDto<string>> UpdateUserRole(int userId, string newRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user's details.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="UserDetailsDto"/></returns>
    Task<ResultWrapperDto<UserDetailsDto>> GetUserDetails(int userId, CancellationToken cancellationToken = default);
}
