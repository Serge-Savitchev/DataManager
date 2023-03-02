using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;
using System.Data;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of <see cref="IUsersService"/>
/// </summary>
public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUsersRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    public UsersService(IUsersRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto>> DeleteUser(int userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.DeleteUserAsync(userId, cancellationToken);
        var ret = ConvertWrapper(result);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto[]>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAllUsersAsync(cancellationToken);
        var ret = ConvertWrapper(result);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto>> GetUser(int userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetUserAsync(userId, cancellationToken);
        var ret = ConvertWrapper(result);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDto[]>> GetUsersByRole(string role,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetUsersByRoleAsync(Enum.Parse<RoleIds>(role, true),
            cancellationToken);
        var ret = ConvertWrapper(result);
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> UpdateOwners(UpdateOwnerRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.UpdateOwnersAsync(request.OwnerId, request.UserIds,
            cancellationToken);
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
