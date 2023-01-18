using DataManagerAPI.Repository.Abstractions.gRPCInterfaces;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Repository.gRPCClients;

public class gRPCUserDataClient : IUserDataRepository
{
    private readonly IgRPCUserDataRepository _igRPCUserDataRepository;

    public gRPCUserDataClient(IgRPCUserDataRepository igRPCUserDataRepository)
    {
        _igRPCUserDataRepository = igRPCUserDataRepository;
    }

    public Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd)
    {
        return _igRPCUserDataRepository.AddUserData(userDataToAdd);
    }

    public Task<ResultWrapper<UserData>> DeleteUserData(int userDataId)
    {
        return _igRPCUserDataRepository.DeleteUserData(userDataId);
    }

    public Task<ResultWrapper<UserData>> GetUserData(int userDataId)
    {
        return _igRPCUserDataRepository.GetUserData(userDataId);
    }

    public Task<ResultWrapper<List<UserData>>> GetUserDataByUserId(int userId)
    {
        return _igRPCUserDataRepository.GetUserDataByUserId(userId);
    }

    public Task<ResultWrapper<UserData>> UpdateUserData(UserData userDataToUpdate)
    {
        return _igRPCUserDataRepository.UpdateUserData(userDataToUpdate);
    }
}
