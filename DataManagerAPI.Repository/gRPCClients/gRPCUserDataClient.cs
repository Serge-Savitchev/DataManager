using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace DataManagerAPI.Repository.gRPCClients;

public class gRPCUserDataClient : IUserDataRepository
{
    private readonly IgRPCUserDataRepository _igRPCUserDataRepository;

    public gRPCUserDataClient(GrpcChannel channel)
    {
        _igRPCUserDataRepository = channel.CreateGrpcService<IgRPCUserDataRepository>();
    }

    public Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd)
    {
        return _igRPCUserDataRepository.AddUserDataAsync(userDataToAdd);
    }

    public Task<ResultWrapper<UserData>> DeleteUserDataAsync(int userDataId)
    {
        return _igRPCUserDataRepository.DeleteUserDataAsync(new UserIdRequest { UserId = userDataId });
    }

    public Task<ResultWrapper<UserData>> GetUserDataAsync(int userDataId)
    {
        return _igRPCUserDataRepository.GetUserDataAsync(new UserIdRequest { UserId = userDataId });
    }

    public Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(int userId)
    {
        return _igRPCUserDataRepository.GetUserDataByUserIdAsync(new UserIdRequest { UserId = userId });
    }

    public Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate)
    {
        return _igRPCUserDataRepository.UpdateUserDataAsync(userDataToUpdate);
    }
}
