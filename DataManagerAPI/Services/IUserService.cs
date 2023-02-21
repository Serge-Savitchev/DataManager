using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;

namespace DataManagerAPI.Services;

/// <summary>
/// Interface for users management service.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Delets user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>Deleted user. <see cref="UserDto"/></returns>
    Task<ResultWrapper<UserDto>> DeleteUser(int userId);

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns><see cref="UserDto"/></returns>
    Task<ResultWrapper<UserDto>> GetUser(int userId);

    /// <summary>
    /// Gets users by role.
    /// </summary>
    /// <param name="role">Role name. <see cref="string"/></param>
    /// <returns>Array of found users. <see cref="UserDto"/></returns>
    Task<ResultWrapper<UserDto[]>> GetUsersByRole(string role);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>Array of all users. <see cref="UserDto"/></returns>
    Task<ResultWrapper<UserDto[]>> GetAllUsers();

    /// <summary>
    /// Updates owner of users.
    /// </summary>
    /// <param name="request"><see cref="UpdateOwnerRequestDto"/></param>
    /// <returns>Array of Ids of users with changed owner</returns>
    Task<ResultWrapper<int>> UpdateOwners(UpdateOwnerRequestDto request);
}
