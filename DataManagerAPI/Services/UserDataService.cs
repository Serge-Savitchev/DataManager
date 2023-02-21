using AutoMapper;
using DataManagerAPI.Dto;
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
    public async Task<ResultWrapper<UserDataDto>> AddUserData(AddUserDataDto userDataToAdd)
    {
        var result = await _repository.AddUserDataAsync(_mapper.Map<UserData>(userDataToAdd));

        var ret = ConvertWrapper(result);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> DeleteUserData(int userDataId)
    {
        var result = await _repository.DeleteUserDataAsync(userDataId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserDataDto>> GetUserData(int userDataId)
    {
        var result = await _repository.GetUserDataAsync(userDataId);

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
    public async Task<ResultWrapper<UserDataDto>> UpdateUserData(UserDataDto userDataToUpdate)
    {
        var result = await _repository.UpdateUserDataAsync(_mapper.Map<UserData>(userDataToUpdate));

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
