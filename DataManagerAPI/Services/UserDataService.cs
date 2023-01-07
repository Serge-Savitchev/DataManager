using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using System.Data;

namespace DataManagerAPI.Services;

public class UserDataService : IUserDataService
{
    private readonly IUserDataRepository _repository;
    private readonly IMapper _mapper;

    public UserDataService(IUserDataRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ResultWrapper<UserDataDto>> AddUserData(AddUserDataDto userDataToAdd)
    {
        var result = await _repository.AddUserData(_mapper.Map<UserData>(userDataToAdd));

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<UserDataDto>> DeleteUserData(int userDataId)
    {
        var result = await _repository.DeleteUserData(userDataId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<UserDataDto>> GetUserData(int userDataId)
    {
        var result = await _repository.GetUserData(userDataId);

        var ret = ConvertWrapper(result);

        return ret;
    }

    public async Task<ResultWrapper<List<UserDataDto>>> GetUserDataByUserId(int userId)
    {
        var result = await _repository.GetUserDataByUserId(userId);
        var ret = new ResultWrapper<List<UserDataDto>>()
        {
            Success = result.Success,
            Data = result.Success ? result.Data!.Select(_mapper.Map<UserDataDto>).ToList() : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        return ret;
    }

    public async Task<ResultWrapper<UserDataDto>> UpdateUserData(UserDataDto userDataToUpdate)
    {
        var result = await _repository.UpdateUserData(_mapper.Map<UserData>(userDataToUpdate));

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
