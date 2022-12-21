﻿using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Models;
using DataManagerAPI.Repository;

namespace DataManagerAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResultWrapper<UserDto>> AddUser(AddUserDto userToAdd)
    {
        var result = await _repository.AddUser(_mapper.Map<User>(userToAdd));

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<UserDto>> DeleteUser(int userId)
    {
        var result = await _repository.DeleteUser(userId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<List<UserDto>>> GetAllUsers()
    {
        var result = await _repository.GetAllUsers();
        var ret = new ResultWrapper<List<UserDto>>()
        {
            Success = result.Success,
            Data = result.Success ? result.Data!.Select(_mapper.Map<UserDto>).ToList() : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        return ret;
    }

    public async Task<ResultWrapper<UserDto>> GetUser(int userId)
    {
        var result = await _repository.GetUser(userId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<List<UserDto>>> GetUsersByRole(string role)
    {
        var result = await _repository.GetUsersByRole(Enum.Parse<RoleId>(role, true));
        var ret = new ResultWrapper<List<UserDto>>()
        {
            Success = result.Success,
            Data = result.Success ? result.Data!.Select(_mapper.Map<UserDto>).ToList() : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        return ret;
    }

    public async Task<ResultWrapper<UserDto>> UpdateUser(UserDto userToUpdate)
    {
        var result = await _repository.UpdateUser(_mapper.Map<User>(userToUpdate));

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<string>> UpdateUserCredentials(int userId, string newLogin, string newPassword)
    {
        var credentials = CredentialsHelper.CreatePasswordHash(newPassword);
        credentials!.Login = newLogin;

        var result = await _repository.UpdateUserCredentials(userId, credentials);

        return new ResultWrapper<string>
        {
            Success = result.Success,
            StatusCode = result.StatusCode,
            Data = result?.Data != null ? result?.Data.UserCredentials.Login : null,
            Message = result?.Message
        };
    }

    public async Task<ResultWrapper<string>> GetUserCredentials(int userId)
    {
        var result = await _repository.GetUserCredentials(userId);

        return new ResultWrapper<string>
        {
            Success = result.Success,
            StatusCode = result.StatusCode,
            Data = result?.Data != null ? result?.Data.Login : null,
            Message = result?.Message
        };
    }

    private ResultWrapper<UserDto> ConvertWrapper<T>(ResultWrapper<T> source)
    {
        var ret = new ResultWrapper<UserDto>
        {
            Success = source.Success,
            Data = source.Success ? _mapper.Map<UserDto>(source.Data) : null,
            Message = source.Message,
            StatusCode = source.StatusCode
        };
        return ret;
    }
}
