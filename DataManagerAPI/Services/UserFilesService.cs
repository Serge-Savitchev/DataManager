﻿using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Dto.Interfaces;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

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
    public async Task<ResultWrapperDto<int>> DeleteFile(CurrentUserDto? currentUser, int userDataId, int fileId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId},userDataId:{userDataId},fileId:{fileId}", currentUser?.User?.Id, userDataId, fileId);

        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            var ret = new ResultWrapperDto<int> { StatusCode = permissions };
            _logger.LogWarning("Finished:{StatusCode},userId:{userId},userDataId:{userDataId},fileId:{fileId}",
                ret.StatusCode, currentUser?.User?.Id, userDataId, fileId);
            return ret;
        }

        var result = await _repository.DeleteFileAsync(userDataId, fileId, cancellationToken);

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId},fileId:{fileId}",
            result.StatusCode, currentUser?.User?.Id, userDataId, fileId);

        return new ResultWrapperDto<int>
        {
            StatusCode = result.StatusCode,
            Success = result.Success,
            Data = result.Data,
            Message = result.Message
        };
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserFileStreamDto>> DownloadFile(CurrentUserDto? currentUser, int userDataId, int fileId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId},userDataId:{userDataId},fileId:{fileId}", currentUser?.User?.Id, userDataId, fileId);

        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            var ret = new ResultWrapperDto<UserFileStreamDto> { StatusCode = permissions };
            _logger.LogWarning("Finished:{StatusCode},userId:{userId},userDataId:{userDataId},fileId:{fileId}",
                ret.StatusCode, currentUser?.User?.Id, userDataId, fileId);

            return ret;
        }

        ResultWrapper<UserFileStream> res = await _repository.DownloadFileAsync(userDataId, fileId, cancellationToken);
        var result = new ResultWrapperDto<UserFileStreamDto>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data != null ? _mapper.Map<UserFileStreamDto>(res.Data) : null
        };

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId},fileId:{fileId},name:{name}",
            result.StatusCode, currentUser?.User?.Id, userDataId, fileId, result.Data?.Name);

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserFileDto[]>> GetList(CurrentUserDto? currentUser, int userDataId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId},userDataId:{userDataId}", currentUser?.User?.Id, userDataId);

        var permissions = await CheckPermissions(currentUser, userDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            var ret = new ResultWrapperDto<UserFileDto[]> { StatusCode = permissions };
            _logger.LogWarning("Finished:{StatusCode},userId:{userId},userDataId:{userDataId}",
                ret.StatusCode, currentUser?.User?.Id, userDataId);

            return ret;
        }

        ResultWrapper<UserFile[]> res = await _repository.GetListAsync(userDataId, cancellationToken);

        var result = new ResultWrapperDto<UserFileDto[]>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data?.Select(x => _mapper.Map<UserFileDto>(x)).ToArray()
        };

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId},length:{length}",
            result.StatusCode, currentUser?.User?.Id, userDataId, result.Data?.Length);

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserFileDto>> UploadFile(CurrentUserDto? currentUser, UserFileStreamDto fileStream,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId},userDataId:{userDataId},fileId:{fileId},name:{name}",
            currentUser?.User?.Id, fileStream.UserDataId, fileStream.Id, fileStream.Name);

        var permissions = await CheckPermissions(currentUser, fileStream.UserDataId, cancellationToken);

        if (permissions != ResultStatusCodes.Status200OK)
        {
            var ret = new ResultWrapperDto<UserFileDto> { StatusCode = permissions };
            _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId},fileId:{fileId},name:{name}",
                ret.StatusCode, currentUser?.User?.Id, fileStream.UserDataId, fileStream.Id, fileStream.Name);
            return ret;
        }

        ResultWrapper<UserFile> res = await _repository.UploadFileAsync(_mapper.Map<UserFileStream>(fileStream), cancellationToken);

        var result = new ResultWrapperDto<UserFileDto>
        {
            Success = res.Success,
            StatusCode = res.StatusCode,
            Message = res.Message,
            Data = res.Data != null ? _mapper.Map<UserFileDto>(res.Data) : null
        };

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId},fileId:{fileId},name:{name}",
            result.StatusCode, currentUser?.User?.Id, fileStream.UserDataId, fileStream.Id, result.Data?.Name);

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
