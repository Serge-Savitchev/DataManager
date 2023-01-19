using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

public class gRPCUserDataRepository : IgRPCUserDataRepository
{
    private readonly IUserDataRepository _repository;

    public gRPCUserDataRepository(IUserDataRepository repository)
    {
        _repository = repository;
    }

    public Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd, CallContext context = default)
    {
        return _repository.AddUserData(userDataToAdd);
    }

    public Task<ResultWrapper<UserData>> DeleteUserData(UserIdRequest userDataId, CallContext context = default)
    {
        return _repository.DeleteUserData(userDataId.UserId);
    }

    public Task<ResultWrapper<UserData>> GetUserData(UserIdRequest userDataId, CallContext context = default)
    {
        return _repository.GetUserData(userDataId.UserId);
    }

    public Task<ResultWrapper<List<UserData>>> GetUserDataByUserId(UserIdRequest userId, CallContext context = default)
    {
        return _repository.GetUserDataByUserId(userId.UserId);
    }

    public Task<ResultWrapper<UserData>> UpdateUserData(UserData userDataToUpdate, CallContext context = default)
    {
        return _repository.UpdateUserData(userDataToUpdate);
    }
}
