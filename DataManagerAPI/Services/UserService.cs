using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using System.Data;

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

    public async Task<ResultWrapper<UserDto>> DeleteUser(int userId)
    {
        var result = await _repository.DeleteUser(userId);
        var ret = ConvertWrapper(result);
        return ret;
    }

    public async Task<ResultWrapper<List<UserDto>>> GetAllUsers()
    {
        var result = await _repository.GetAllUsers();
        var ret = ConvertWrapper(result);
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
        var result = await _repository.GetUsersByRole(Enum.Parse<RoleIds>(role, true));
        var ret = ConvertWrapper(result);
        return ret;
    }

    public async Task<ResultWrapper<int>> UpdateOwners(UpdateOwnerRequest request)
    {
        var result = await _repository.UpdateOwners(request.OwnerId, request.UserIds);
        return result;
    }

    private ResultWrapper<UserDto> ConvertWrapper(ResultWrapper<User> source)
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

    private ResultWrapper<List<UserDto>> ConvertWrapper(ResultWrapper<List<User>> source)
    {
        var ret = new ResultWrapper<List<UserDto>>()
        {
            Success = source.Success,
            Data = source.Success && source.Data != null ? source.Data.Select(_mapper.Map<UserDto>).ToList() : null,
            Message = source.Message,
            StatusCode = source.StatusCode
        };

        return ret;
    }
}
