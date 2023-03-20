namespace DataManagerAPI.Dto.Interfaces;

/// <summary>
/// Interface of UserData service.
/// </summary>
public interface IUserDataService
{
    /// <summary>
    /// Adds new User Data.
    /// </summary>
    /// <param name="userId">Id of User</param>
    /// <param name="userDataToAdd"><see cref="AddUserDataDto"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>New user data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapperDto<UserDataDto>> AddUserData(int userId, AddUserDataDto userDataToAdd,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates User Data.
    /// </summary>
    /// <param name="userId">Id of User</param>
    /// <param name="userDataId">Id of User Data</param>
    /// <param name="userDataToUpdate"> <see cref="AddUserDataDto"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Updated user data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapperDto<UserDataDto>> UpdateUserData(int userId, int userDataId, AddUserDataDto userDataToUpdate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes User Data by Id.
    /// </summary>
    /// <param name="userId">Id of User</param>
    /// <param name="userDataId">Id of User Data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Deleted User Data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapperDto<UserDataDto>> DeleteUserData(int userId, int userDataId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets User Data by Id.
    /// </summary>
    /// <param name="userId">Id of User</param>
    /// <param name="userDataId">Id of User Data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>User Data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapperDto<UserDataDto>> GetUserData(int userId, int userDataId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all User Data by user Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of User Data. <see cref="UserDataDto"/></returns>
    Task<ResultWrapperDto<UserDataDto[]>> GetUserDataByUserId(int userId,
        CancellationToken cancellationToken = default);
}
