using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;

namespace DataManagerAPI.Services.Interfaces;

/// <summary>
/// Interface for users management service.
/// </summary>
public interface IUsersService
{
    /// <summary>
    /// Delets user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Deleted user. <see cref="UserDto"/></returns>
    Task<ResultWrapper<UserDto>> DeleteUser(int userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="UserDto"/></returns>
    Task<ResultWrapper<UserDto>> GetUser(int userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by role.
    /// </summary>
    /// <param name="role">Role name. <see cref="string"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of found users. <see cref="UserDto"/></returns>
    Task<ResultWrapper<UserDto[]>> GetUsersByRole(string role,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of all users. <see cref="UserDto"/></returns>
    Task<ResultWrapper<UserDto[]>> GetAllUsers(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates owner of users.
    /// </summary>
    /// <param name="request"><see cref="UpdateOwnerRequestDto"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Count of users with changed owner</returns>
    Task<ResultWrapper<int>> UpdateOwners(UpdateOwnerRequestDto request,
        CancellationToken cancellationToken = default);
}
