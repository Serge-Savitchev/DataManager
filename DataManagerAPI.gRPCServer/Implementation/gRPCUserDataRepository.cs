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

    public Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd, CallContext context = default)
    {
        return _repository.AddUserDataAsync(userDataToAdd);
    }

    public Task<ResultWrapper<UserData>> DeleteUserDataAsync(UserIdRequest userDataId, CallContext context = default)
    {
        return _repository.DeleteUserDataAsync(userDataId.UserId);
    }

    public Task<ResultWrapper<UserData>> GetUserDataAsync(UserIdRequest userDataId, CallContext context = default)
    {
        return _repository.GetUserDataAsync(userDataId.UserId);
    }

    public Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(UserIdRequest userId, CallContext context = default)
    {
        return _repository.GetUserDataByUserIdAsync(userId.UserId);
    }

    public Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate, CallContext context = default)
    {
        return _repository.UpdateUserDataAsync(userDataToUpdate);
    }
}
