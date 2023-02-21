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
    /// <param name="userToAdd">The <see cref="User"/> defines user's data</param>
    /// <param name="userCredentials">The <see cref="UserCredentials"/> defines user's credentials</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>A task that represents the result <see cref="User"/> of asynchronous operation</returns>
    [OperationContract]
    Task<ResultWrapper<User>> RegisterUserAsync(User userToAdd, UserCredentials userCredentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates and authorizes user.
    /// </summary>
    /// <param name="login">Specifies user login</param>
    /// <param name="credentials">The <see cref="UserCredentials"/> defines user's credentials</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>User Id</returns>
    [OperationContract]
    Task<ResultWrapper<int>> LoginAsync(string login, UserCredentials credentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores new Refresh token to database.
    /// </summary>
    /// <param name="userId">Specifies user Id</param>
    /// <param name="refreshToken">Specifies new Refresh token. It can be null</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>User Id</returns>
    [OperationContract]
    Task<ResultWrapper<int>> RefreshTokenAsync(int userId, string? refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get stored in database user's credentials.
    /// </summary>
    /// <param name="userId">Specifies user Id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>A task that represents the result <see cref="UserCredentialsData"/> of asynchronous operation</returns>
    [OperationContract]
    Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches user in database by login.
    /// </summary>
    /// <param name="login">Specifies user login</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>A task that represents the result <see cref="UserCredentialsData"/> of asynchronous operation</returns>
    [OperationContract]
    Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(string login, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user password in database.
    /// </summary>
    /// <param name="userId">Specifies user Id</param>
    /// <param name="credentials">The <see cref="UserCredentials"/> defines user's credentials</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>User Id</returns>
    [OperationContract]
    Task<ResultWrapper<int>> UpdateUserPasswordAsync(int userId, UserCredentials credentials, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user role in database.
    /// </summary>
    /// <param name="userId">Specifies user Id</param>
    /// <param name="newRole">Specifies new role <see cref="RoleIds"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>A task that represents the result <see cref="RoleIds"/> of asynchronous operation</returns>
    [OperationContract]
    Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(int userId, RoleIds newRole, CancellationToken cancellationToken = default);
}
