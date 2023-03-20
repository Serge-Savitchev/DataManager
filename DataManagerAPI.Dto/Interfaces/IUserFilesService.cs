namespace DataManagerAPI.Dto.Interfaces;

/// <summary>
/// Interface for accessing files in database.
/// </summary>
public interface IUserFilesService
{
    /// <summary>
    /// Returns list of all files in database for userdata.<see cref="UserDataDto"/>
    /// </summary>
    /// <param name="currentUser"><see cref="CurrentUserDto"/></param>
    /// <param name="userDataId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of files. <see cref="UserFileDto"/></returns>
    Task<ResultWrapperDto<UserFileDto[]>> GetList(CurrentUserDto? currentUser, int userDataId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes file from database.
    /// </summary>
    /// <param name="currentUser"><see cref="CurrentUserDto"/></param>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of deleted file</returns>
    Task<ResultWrapperDto<int>> DeleteFile(CurrentUserDto? currentUser, int userDataId, int fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads file from database.
    /// </summary>
    /// <param name="currentUser"><see cref="CurrentUserDto"/></param>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Stream for file downloading. <see cref="UserFileStreamDto"/></returns>
    Task<ResultWrapperDto<UserFileStreamDto>> DownloadFile(CurrentUserDto? currentUser, int userDataId, int fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads file to database.
    /// </summary>
    /// <param name="currentUser"><see cref="CurrentUserDto"/></param>
    /// <param name="fileStream"><see cref="UserFileStreamDto"/></param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="UserFileDto"/></returns>
    Task<ResultWrapperDto<UserFileDto>> UploadFile(CurrentUserDto? currentUser, UserFileStreamDto fileStream, CancellationToken cancellationToken = default);
}
