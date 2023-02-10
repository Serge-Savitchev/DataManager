using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using System.ServiceModel;

namespace DataManagerAPI.Repository.Abstractions.Interfaces;

/// <summary>
/// Interface for accessing files in database.
/// </summary>
[ServiceContract]
public interface IUserFileRepository
{
    /// <summary>
    /// Returns list of all files in database for userdata. <see cref="UserData"/>.
    /// </summary>
    /// <param name="userDataId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Array of files. <see cref="File"/>.</returns>
    Task<ResultWrapper<UserFile[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default);

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
    Task<ResultWrapper<UserFileStream>> DownloadFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads file to database.
    /// </summary>
    /// <param name="fileStream"><see cref="UserFileStream"/>.</param>
    /// <param name="cancellationToken"></param>
    /// <returns><see cref="UserFile"/></returns>
    Task<ResultWrapper<UserFile>> UploadFileAsync(UserFileStream fileStream, CancellationToken cancellationToken = default);
}
