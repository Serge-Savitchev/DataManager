using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Helpers;
using DataManagerAPI.Models;
using DataManagerAPI.Repository;

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
        var ret = new ResultWrapper<UserDataDto>
        {
            Data = result.Success ? _mapper.Map<UserDataDto>(result.Data) : null,
            Message = result.Message,
            StatusCode = result.StatusCode
        };
        return ret;
    }
}
