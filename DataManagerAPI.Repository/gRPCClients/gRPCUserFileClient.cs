using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Repository.gRPCClients;

public class gRPCUserFileClient : IUserFileRepository
{
    public Task<ResultWrapper<int>> DeleteFileAsync(int userDataId, int Id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultWrapper<UserFileStream>> DownloadFileAsync(int userDataId, int Id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultWrapper<UserFile[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultWrapper<UserFile>> UploadFileAsync(UserFileStream fileStream, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
