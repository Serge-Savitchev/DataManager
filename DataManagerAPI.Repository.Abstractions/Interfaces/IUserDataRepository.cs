using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

/// <summary>
/// Interface of managing UserData in database.
/// </summary>
[ServiceContract]
public interface IUserDataRepository
{
    /// <summary>
    /// Adds new User Data.
    /// </summary>
    /// <param name="userDataToAdd"><see cref="UserData"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>New user data. <see cref="UserData"/></returns>
    [OperationContract]
    Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates User Data.
    /// </summary>
    /// <param name="userDataToUpdate"> <see cref="UserData"/></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Updated user data. <see cref="UserData"/></returns>
    [OperationContract]
    Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes User Data by Id.
    /// </summary>
    /// <param name="userId">Id of User</param>
    /// <param name="userDataId">Id of User Data</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Deleted User Data. <see cref="UserData"/></returns>
    [OperationContract]
    Task<ResultWrapper<UserData>> DeleteUserDataAsync(int userId, int userDataId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets User Data by Id.
    /// </summary>
    /// <param name="userId">Id of User</param>
    /// <param name="userDataId">Id of User Data</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>User Data. <see cref="UserData"/></returns>
    [OperationContract]
    Task<ResultWrapper<UserData>> GetUserDataAsync(int userId, int userDataId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all User Data by user Id.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>Array of User Data. <see cref="UserData"/></returns>
    [OperationContract]
    Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns owning user of UserData by UserData Id.
    /// </summary>
    /// <param name="userDataId">Id of user data</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns><see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User>> GetUserAsync(int userDataId, CancellationToken cancellationToken = default);
}
