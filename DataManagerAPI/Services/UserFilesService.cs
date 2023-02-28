using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of <see cref="IUserFilesService"/>.
/// </summary>
public class UserFilesService : IUserFilesService
{
    private readonly IUserFilesRepository _repository;
    private readonly IUserDataRepository _dataRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserFilesRepository"/></param>
    /// <param name="dataRepository"><see cref="IUserDataRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    public UserFilesService(IUserFilesRepository repository, IUserDataRepository dataRepository, IMapper mapper)
    {
        _repository = repository;
        _dataRepository = dataRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> DeleteFileAsync(CurrentUserDto? currentUser, int userDataId, int fileId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            return new ResultWrapper<int>
            {
                StatusCode = permissions
            };
        }

        return await _repository.DeleteFileAsync(userDataId, fileId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileStreamDto>> DownloadFileAsync(CurrentUserDto? currentUser, int userDataId, int fileId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            return new ResultWrapper<UserFileStreamDto>
            {
                StatusCode = permissions
            };
        }

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
    public async Task<ResultWrapper<UserFileDto[]>> GetListAsync(CurrentUserDto? currentUser, int userDataId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            return new ResultWrapper<UserFileDto[]>
            {
                StatusCode = permissions
            };
        }

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
    public async Task<ResultWrapper<UserFileDto>> UploadFileAsync(CurrentUserDto? currentUser, UserFileStreamDto fileStream,
        CancellationToken cancellationToken = default)
    {
        var permissions = await CheckPermissions(currentUser, fileStream.UserDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            return new ResultWrapper<UserFileDto>
            {
                StatusCode = permissions
            };
        }

        ResultWrapper<UserFile> res = await _repository.UploadFileAsync(_mapper.Map<UserFileStream>(fileStream), cancellationToken);
        return new ResultWrapper<UserFileDto>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data != null ? _mapper.Map<UserFileDto>(res.Data) : null
        };
    }

    #region Helpers

    private async Task<int> CheckPermissions(CurrentUserDto? currentUser, int userDataId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser == null)
        {
            return ResultStatusCodes.Status401Unauthorized;
        }

        var role = Enum.Parse<RoleIds>(currentUser.User!.Role, true);

        if (role == RoleIds.Admin)
        {
            return ResultStatusCodes.Status200OK;
        }

        ResultWrapper<User> user = await _dataRepository.GetUserAsync(userDataId, cancellationToken);
        if (!user.Success)
        {
            return ResultStatusCodes.Status400BadRequest;
        }

        if (user.Data!.Id != currentUser.User.Id)
        {
            return ResultStatusCodes.Status403Forbidden;
        }

        return ResultStatusCodes.Status200OK;
    }

    #endregion
}
