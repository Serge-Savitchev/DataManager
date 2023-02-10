using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;

namespace DataManagerAPI.Services;

/// <summary>
/// Interface for accessing files in database.
/// </summary>
public interface IUserFileService
{
    /// <summary>
    /// Returns list of all files in database for userdata.<see cref="UserDataDto"/>
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of files. <see cref="UserFileDto"/>.</returns>
    Task<ResultWrapper<UserFileDto[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes file from database.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of deleted file</returns>
    Task<ResultWrapper<int>> DeleteFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads file from database.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Stream for file downloading. <see cref="UserFileStream"/>.</returns>
    Task<ResultWrapper<UserFileStreamDto>> DownloadFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads file to database.
    /// </summary>
    /// <param name="fileStream"><see cref="UserFileStreamDto"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="UserFileDto"/></returns>
    Task<ResultWrapper<UserFileDto>> UploadFileAsync(UserFileStreamDto fileStream, CancellationToken cancellationToken = default);
}
