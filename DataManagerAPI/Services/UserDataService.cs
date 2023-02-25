using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;
using System.Data;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of IUserDataService.
/// </summary>
public class UserDataService : IUserDataService
{
    private readonly IUserDataRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserDataRepository"/></param>
    /// <param name="mapper"><see cref="IMapper"/></param>
    public UserDataService(IUserDataRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> AddUserData(int userId, int userDataId, AddUserDataDto userDataToAdd)
    {
        var userData = _mapper.Map<UserData>(userDataToAdd);
        userData.UserId = userId;
        userData.Id = 0;

        var result = await _repository.AddUserDataAsync(userData);

        var ret = ConvertWrapper(result);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> DeleteUserData(int userId, int userDataId)
    {
        var result = await _repository.DeleteUserDataAsync(userId, userDataId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> GetUserData(int userId, int userDataId)
    {
        var result = await _repository.GetUserDataAsync(userId, userDataId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto[]>> GetUserDataByUserId(int userId)
    {
        var result = await _repository.GetUserDataByUserIdAsync(userId);
        var ret = new ResultWrapper<UserDataDto[]>()
        {
            Success = result.Success,
            Data = result.Success ? result.Data!.Select(_mapper.Map<UserDataDto>).ToArray() : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> UpdateUserData(int userId, int userDataId, AddUserDataDto userDataToUpdate)
    {
        var userData = _mapper.Map<UserData>(userDataToUpdate);
        userData.UserId = userId;
        userData.Id = userDataId;

        var result = await _repository.UpdateUserDataAsync(userData);

        var ret = ConvertWrapper(result);

        return ret;
    }

    private ResultWrapper<UserDataDto> ConvertWrapper<T>(ResultWrapper<T> source)
    {
        var ret = new ResultWrapper<UserDataDto>
        {
            Success = source.Success,
            Data = source.Success ? _mapper.Map<UserDataDto>(source.Data) : null,
            Message = source.Message,
            StatusCode = source.StatusCode
        };
        return ret;
    }
}
