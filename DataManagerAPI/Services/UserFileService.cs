using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of <see cref="IUserFileService"/>.
/// </summary>
public class UserFileService : IUserFileService
{
    private readonly IUserFilesRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserFilesRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    public UserFileService(IUserFilesRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> DeleteFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteFileAsync(userDataId, fileId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileStreamDto>> DownloadFileAsync(int userDataId, int fileId, CancellationToken cancellationToken = default)
    {
        ResultWrapper<UserFileStream> res = await _repository.DownloadFileAsync(userDataId, fileId, cancellationToken);
        return new ResultWrapper<UserFileStreamDto>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data != null ? _mapper.Map<UserFileStreamDto>(res.Data) : null
        };
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileDto[]>> GetListAsync(int userDataId, CancellationToken cancellationToken = default)
    {
        ResultWrapper<UserFile[]> res = await _repository.GetListAsync(userDataId, cancellationToken);
        return new ResultWrapper<UserFileDto[]>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data?.Select(x => _mapper.Map<UserFileDto>(x)).ToArray()
        };
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileDto>> UploadFileAsync(UserFileStreamDto fileStream, CancellationToken cancellationToken = default)
    {
        ResultWrapper<UserFile> res = await _repository.UploadFileAsync(_mapper.Map<UserFileStream>(fileStream), cancellationToken);
        return new ResultWrapper<UserFileDto>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data != null ? _mapper.Map<UserFileDto>(res.Data) : null
        };
    }
}
