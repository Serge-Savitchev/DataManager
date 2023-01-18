using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

/// <summary>
/// This interface provides users management in database.
/// </summary>
[ServiceContract]
public interface IAuthRepository
{
    /// <summary>
    /// Register new user in database.
    /// </summary>
    /// <param name="userToAdd">The <see cref="User"/> defines user's data.</param>
    /// <param name="userCredentials">The <see cref="UserCredentials"/> defines user's credentials.</param>
    /// <returns>A task that represents the result <see cref="ResultWrapper<User>"/> of asynchronous operation.</returns>
    [OperationContract]
    Task<ResultWrapper<User>> RegisterUserAsync(User userToAdd, UserCredentials userCredentials);

    /// <summary>
    /// Authenticates and authorizes user.
    /// </summary>
    /// <param name="login">Specifies user login.</param>
    /// <param name="credentials">The <see cref="UserCredentials"/> defines user's credentials.</param>
    /// <returns>A task that represents the result <see cref="ResultWrapper<int>"/> of asynchronous operation.</returns>
    [OperationContract]
    Task<ResultWrapper<int>> LoginAsync(string login, UserCredentials credentials);

    /// <summary>
    /// Stores new Refresh token to database.
    /// </summary>
    /// <param name="userId">Specifies user Id.</param>
    /// <param name="refreshToken">Specifies new Refresh token. It can be null.</param>
    /// <returns>A task that represents the result <see cref="ResultWrapper<int>"/> of asynchronous operation.</returns>
    [OperationContract]
    Task<ResultWrapper<int>> RefreshTokenAsync(int userId, string? refreshToken);

    /// <summary>
    /// Returns stored in database user's credentials.
    /// </summary>
    /// <param name="userId">Specifies user Id.</param>
    /// <returns>A task that represents the result <see cref="ResultWrapper<UserCredentialsData>"/> of asynchronous operation.</returns>
    [OperationContract]
    Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(int userId);

    /// <summary>
    /// Searches user in database by login.
    /// </summary>
    /// <param name="login">Specifies user login.</param>
    /// <returns>A task that represents the result <see cref="ResultWrapper<UserCredentialsData>"/> of asynchronous operation.</returns>
    [OperationContract]
    Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(string login);

    /// <summary>
    /// Updates user password in database.
    /// </summary>
    /// <param name="userId">Specifies user Id.</param>
    /// <param name="credentials">The <see cref="UserCredentials"/> defines user's credentials.</param>
    /// <returns>A task that represents the result <see cref="ResultWrapper<int>"/> of asynchronous operation.</returns>
    [OperationContract]
    Task<ResultWrapper<int>> UpdateUserPasswordAsync(int userId, UserCredentials credentials);

    /// <summary>
    /// Update user role in database.
    /// </summary>
    /// <param name="userId">Specifies user Id.</param>
    /// <param name="newRole">Specifies new role <see cref="RoleIds"/>.</param>
    /// <returns>A task that represents the result <see cref="ResultWrapper<RoleIds>"/> of asynchronous operation.</returns>
    [OperationContract]
    Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(int userId, RoleIds newRole);
}
