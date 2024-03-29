﻿using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Dto.Interfaces;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using System.Data;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of <see cref="IUsersService"/>
/// </summary>
public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersService> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUsersRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    /// <param name="logger"><see cref="ILogger"/></param>
    public UsersService(IUsersRepository repository, IMapper mapper, ILogger<UsersService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDto>> DeleteUser(int userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId}", userId);

        var result = await _repository.DeleteUserAsync(userId, cancellationToken);
        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished:{StatusCode},userId:{userId}", ret.StatusCode, userId);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDto[]>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

        var result = await _repository.GetAllUsersAsync(cancellationToken);
        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished:{StatusCode},length:{length}", ret.StatusCode, ret.Data?.Length);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDto>> GetUser(int userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId}", userId);

        var result = await _repository.GetUserAsync(userId, cancellationToken);
        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished:{StatusCode},userId:{userId}", ret.StatusCode, userId);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<UserDto[]>> GetUsersByRole(string role, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:role:{role}", role);

        var result = await _repository.GetUsersByRoleAsync(Enum.Parse<RoleIds>(role, true),
            cancellationToken);
        var ret = ConvertWrapper(result);

        _logger.LogInformation("Finished:{StatusCode},role:{role},length:{length}", ret.StatusCode, role, ret.Data?.Length);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapperDto<int>> UpdateOwners(UpdateOwnerRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:userId:{userId}", request.OwnerId);

        var ret = await _repository.UpdateOwnersAsync(request.OwnerId, request.UserIds, cancellationToken);

        _logger.LogInformation("Finished:{StatusCode},userId:{userId},data:{data}", ret.StatusCode, request.OwnerId, ret.Data);

        return new ResultWrapperDto<int>
        {
            StatusCode = ret.StatusCode,
            Success = ret.Success,
            Data = ret.Data,
            Message = ret.Message
        };
    }

    private ResultWrapperDto<UserDto> ConvertWrapper(ResultWrapper<User> source)
    {
        var ret = new ResultWrapperDto<UserDto>
        {
            Success = source.Success,
            Data = source.Success ? _mapper.Map<UserDto>(source.Data) : null,
            Message = source.Message,
            StatusCode = source.StatusCode
        };
        return ret;
    }

    private ResultWrapperDto<UserDto[]> ConvertWrapper(ResultWrapper<User[]> source)
    {
        var ret = new ResultWrapperDto<UserDto[]>()
        {
            Success = source.Success,
            Data = source.Success && source.Data != null ? source.Data.Select(_mapper.Map<UserDto>).ToArray() : null,
            Message = source.Message,
            StatusCode = source.StatusCode
        };

        return ret;
    }
}
