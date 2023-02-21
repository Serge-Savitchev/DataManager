using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;

namespace DataManagerAPI.Services;

/// <summary>
/// Interface of UserData service.
/// </summary>
public interface IUserDataService
{
    /// <summary>
    /// Adds new User Data.
    /// </summary>
    /// <param name="userDataToAdd"><see cref="AddUserDataDto"/></param>
    /// <returns>New user data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapper<UserDataDto>> AddUserData(AddUserDataDto userDataToAdd);

    /// <summary>
    /// Updates User Data.
    /// </summary>
    /// <param name="userDataToUpdate"> <see cref="UserDataDto"/></param>
    /// <returns>Updated user data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapper<UserDataDto>> UpdateUserData(UserDataDto userDataToUpdate);

    /// <summary>
    /// Deletes User Data by Id.
    /// </summary>
    /// <param name="userDataId">Id of User Data</param>
    /// <returns>Deleted User Data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapper<UserDataDto>> DeleteUserData(int userDataId);

    /// <summary>
    /// Gets User Data by Id.
    /// </summary>
    /// <param name="userDataId">Id of User Data</param>
    /// <returns>User Data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapper<UserDataDto>> GetUserData(int userDataId);

    /// <summary>
    /// Gets all User Data by user Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>Array of User Data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapper<UserDataDto[]>> GetUserDataByUserId(int userId);
}
