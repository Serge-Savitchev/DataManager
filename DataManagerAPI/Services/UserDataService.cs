﻿using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Dto.Interfaces;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using System.Data;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of IUserDataService.
/// </summary>
public class UserDataService : IUserDataService
{
    private readonly IUserDataRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserDataService> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserDataRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public UserDataService(IUserDataRepository repository, IMapper mapper, ILogger<UserDataService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDataDto>> AddUserData(int userId, AddUserDataDto userDataToAdd,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId}", userId);

        var userData = _mapper.Map<UserData>(userDataToAdd);
        userData.UserId = userId;
        userData.Id = 0;

        var result = await _repository.AddUserDataAsync(userData, cancellationToken);

        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId}", ret.StatusCode, userId, ret.Data?.Id);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDataDto>> DeleteUserData(int userId, int userDataId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId},userDataId:{userDataId}", userId, userDataId);

        var result = await _repository.DeleteUserDataAsync(userId, userDataId, cancellationToken);

        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId}", ret.StatusCode, userId, userDataId);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDataDto>> GetUserData(int userId, int userDataId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId},userDataId:{userDataId}", userId, userDataId);

        var result = await _repository.GetUserDataAsync(userId, userDataId, cancellationToken);

        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId}", ret.StatusCode, userId, userDataId);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDataDto[]>> GetUserDataByUserId(int userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId}", userId);

        var result = await _repository.GetUserDataByUserIdAsync(userId, cancellationToken);
        var ret = new ResultWrapperDto<UserDataDto[]>()
        {
            Success = result.Success,
            Data = result.Success ? result.Data!.Select(_mapper.Map<UserDataDto>).ToArray() : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},length:{length}", ret.StatusCode, userId, ret.Data?.Length);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDataDto>> UpdateUserData(int userId, int userDataId, AddUserDataDto userDataToUpdate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId},userDataId:{userDataId}", userId, userDataId);

        var userData = _mapper.Map<UserData>(userDataToUpdate);
        userData.UserId = userId;
        userData.Id = userDataId;

        var result = await _repository.UpdateUserDataAsync(userData, cancellationToken);

        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},userDataId:{userDataId}", ret.StatusCode, userId, userDataId);

        return ret;
    }

    private ResultWrapperDto<UserDataDto> ConvertWrapper<T>(ResultWrapper<T> source)
    {
        var ret = new ResultWrapperDto<UserDataDto>
        {
            Success = source.Success,
            Data = source.Success ? _mapper.Map<UserDataDto>(source.Data) : null,
            StatusCode = source.StatusCode
        };
        return ret;
    }
}
