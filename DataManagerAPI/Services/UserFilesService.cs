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
    private readonly ILogger<UserFilesService> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserFilesRepository"/></param>
    /// <param name="dataRepository"><see cref="IUserDataRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public UserFilesService(IUserFilesRepository repository, IUserDataRepository dataRepository,
        IMapper mapper, ILogger<UserFilesService> logger)
    {
        _repository = repository;
        _dataRepository = dataRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> DeleteFile(CurrentUserDto? currentUser, int userDataId, int fileId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId},{fileId}", currentUser?.User?.Id, userDataId, fileId);

        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            var ret = new ResultWrapper<int> { StatusCode = permissions };
            _logger.LogWarning("Finished:{@ret}", ret);
            return ret;
        }

        var result = await _repository.DeleteFileAsync(userDataId, fileId, cancellationToken);

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileStreamDto>> DownloadFile(CurrentUserDto? currentUser, int userDataId, int fileId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId},{fileId}", currentUser?.User?.Id, userDataId, fileId);

        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            var ret = new ResultWrapper<UserFileStreamDto> { StatusCode = permissions };
            _logger.LogWarning("Finished:{@ret}", ret);
            return ret;
        }

        ResultWrapper<UserFileStream> res = await _repository.DownloadFileAsync(userDataId, fileId, cancellationToken);
        var result = new ResultWrapper<UserFileStreamDto>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data != null ? _mapper.Map<UserFileStreamDto>(res.Data) : null
        };

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileDto[]>> GetList(CurrentUserDto? currentUser, int userDataId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId}", currentUser?.User?.Id, userDataId);

        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            var ret = new ResultWrapper<UserFileDto[]> { StatusCode = permissions };
            _logger.LogWarning("Finished:{@ret}", ret);
            return ret;
        }

        ResultWrapper<UserFile[]> res = await _repository.GetListAsync(userDataId, cancellationToken);

        var result = new ResultWrapper<UserFileDto[]>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data?.Select(x => _mapper.Map<UserFileDto>(x)).ToArray()
        };

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserFileDto>> UploadFile(CurrentUserDto? currentUser, UserFileStreamDto fileStream,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId},{fileId}", currentUser?.User?.Id, fileStream.UserDataId, fileStream.Id);

        var permissions = await CheckPermissions(currentUser, fileStream.UserDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            var ret = new ResultWrapper<UserFileDto> { StatusCode = permissions };
            _logger.LogWarning("Finished:{@ret}", ret);
            return ret;
        }

        ResultWrapper<UserFile> res = await _repository.UploadFileAsync(_mapper.Map<UserFileStream>(fileStream), cancellationToken);

        var result = new ResultWrapper<UserFileDto>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data != null ? _mapper.Map<UserFileDto>(res.Data) : null
        };

        _logger.LogInformation("Finished");

        return result;
    }

    #region Helpers

    private async Task<int> CheckPermissions(CurrentUserDto? currentUser, int userDataId,
        CancellationToken cancellationToken = default)
    {
        if (currentUser == null)    // unauthorized request
        {
            return ResultStatusCodes.Status401Unauthorized;
        }

        var role = Enum.Parse<RoleIds>(currentUser.User!.Role, true);

        if (role == RoleIds.Admin)  // admin has access to all files
        {
            return ResultStatusCodes.Status200OK;
        }

        ResultWrapper<User> user = await _dataRepository.GetUserAsync(userDataId, cancellationToken);
        if (!user.Success)  // owner is not found
        {
            return ResultStatusCodes.Status400BadRequest;
        }

        if (user.Data!.Id != currentUser.User.Id)   // access to another's file
        {
            return ResultStatusCodes.Status403Forbidden;
        }

        return ResultStatusCodes.Status200OK;
    }

    #endregion
}
