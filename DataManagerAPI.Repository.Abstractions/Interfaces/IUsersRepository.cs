using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

/// <summary>
/// Interface for managing users in database.
/// </summary>
[ServiceContract]
public interface IUsersRepository
{
    /// <summary>
    /// Deletes user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Deleted user. <see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User>> DeleteUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User>> GetUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by role.
    /// </summary>
    /// <param name="roleId">Role id. <see cref="RoleIds"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Array of found users. <see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleIds roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Array of all users. <see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User[]>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates owner of users.
    /// </summary>
    /// <param name="ownerId">Owner Id</param>
    /// <param name="users">Array of user Ids to be updated</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Count of users with changed owner</returns>
    [OperationContract]
    Task<ResultWrapper<int>> UpdateOwnersAsync(int ownerId, int[] users, CancellationToken cancellationToken = default);
}

