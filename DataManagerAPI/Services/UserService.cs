using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using System.Data;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of <see cref="IUserService"/>
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto>> DeleteUser(int userId)
    {
        var result = await _repository.DeleteUserAsync(userId);
        var ret = ConvertWrapper(result);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto[]>> GetAllUsers()
    {
        var result = await _repository.GetAllUsersAsync();
        var ret = ConvertWrapper(result);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto>> GetUser(int userId)
    {
        var result = await _repository.GetUserAsync(userId);
        var ret = ConvertWrapper(result);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto[]>> GetUsersByRole(string role)
    {
        var result = await _repository.GetUsersByRoleAsync(Enum.Parse<RoleIds>(role, true));
        var ret = ConvertWrapper(result);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> UpdateOwners(UpdateOwnerRequestDto request)
    {
        var result = await _repository.UpdateOwnersAsync(request.OwnerId, request.UserIds);
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

    private ResultWrapper<UserDto[]> ConvertWrapper(ResultWrapper<User[]> source)
    {
        var ret = new ResultWrapper<UserDto[]>()
        {
            Success = source.Success,
            Data = source.Success && source.Data != null ? source.Data.Select(_mapper.Map<UserDto>).ToArray() : null,
            Message = source.Message,
            StatusCode = source.StatusCode
        };

        return ret;
    }
}
