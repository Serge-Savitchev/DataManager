using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;

/// <summary>
/// Interface for UserFilesRepository in gRPC server.
/// It provides access to database via gRPC service. It has the same set of methods as IUserFilesRepository interface has.
/// </summary>
[ServiceContract]
public interface IgRPCUserFilesRepository
{
    /// <summary>
    /// Returns list of all files in database for userdata. <see cref="UserData"/>.
    /// </summary>
    /// <param name="request"><see cref="Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>Array of files. <see cref="UserFile"/></returns>
    Task<ResultWrapper<UserFile[]>> GetListAsync(Int32Request request, CallContext context = default);

    /// <summary>
    /// Deletes file from database.
    /// </summary>
    /// <param name="request"><see cref="Int32Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>Id of deleted file</returns>
    Task<ResultWrapper<int>> DeleteFileAsync(Int32Int32Request request, CallContext context = default);

    /// <summary>
    /// Downloads file from database.
    /// </summary>
    /// <param name="request"><see cref="Int32Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>Stream for file downloading. <see cref="UserFileStream"/></returns>
    Task<ResultWrapper<UserFileStream>> DownloadFileAsync(Int32Int32Request request, CallContext context = default);

    /// <summary>
    /// Uploads file to database.
    /// </summary>
    /// <param name="fileStream"><see cref="UserFileStream"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns><see cref="UserFile"/></returns>
    Task<ResultWrapper<UserFile>> UploadFileAsync(UserFileStream fileStream, CallContext context = default);
}
