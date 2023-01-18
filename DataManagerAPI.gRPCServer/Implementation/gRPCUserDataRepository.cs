using DataManagerAPI.Repository.Abstractions.gRPCInterfaces;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.gRPCServer.Implementation;

public class gRPCUserDataRepository : IgRPCUserDataRepository
{
    private readonly IUserDataRepository _repository;

    public gRPCUserDataRepository(IUserDataRepository repository)
    {
        _repository = repository;
    }

    public Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd)
    {
        return _repository.AddUserData(userDataToAdd);
    }

    public Task<ResultWrapper<UserData>> DeleteUserData(int userDataId)
    {
        return _repository.DeleteUserData(userDataId);
    }

    public Task<ResultWrapper<UserData>> GetUserData(int userDataId)
    {
        return _repository.GetUserData(userDataId);
    }

    public Task<ResultWrapper<List<UserData>>> GetUserDataByUserId(int userId)
    {
        return _repository.GetUserDataByUserId(userId);
    }

    public Task<ResultWrapper<UserData>> UpdateUserData(UserData userDataToUpdate)
    {
        return _repository.UpdateUserData(userDataToUpdate);
    }
}
