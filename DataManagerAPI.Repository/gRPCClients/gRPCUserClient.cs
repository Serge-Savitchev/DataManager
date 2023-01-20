using Azure.Core;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace DataManagerAPI.Repository.gRPCClients;

public class gRPCUserClient : IUserRepository
{
    private readonly IgRPCUserRepository _igRPCUserRepository;

    public gRPCUserClient(GrpcChannel channel)
    {
        _igRPCUserRepository = channel.CreateGrpcService<IgRPCUserRepository>();
    }
    public Task<ResultWrapper<User>> DeleteUser(int userId)
    {
        return _igRPCUserRepository.DeleteUser(new UserIdRequest { UserId = userId });
    }

    public Task<ResultWrapper<List<User>>> GetAllUsers()
    {
        return _igRPCUserRepository.GetAllUsers(new EmptyRequest());
    }

    public Task<ResultWrapper<User>> GetUser(int userId)
    {
        return _igRPCUserRepository.GetUser(new UserIdRequest { UserId = userId });
    }

    public Task<ResultWrapper<List<User>>> GetUsersByRole(RoleIds roleId)
    {
        return _igRPCUserRepository.GetUsersByRole(new RoleRequest { RoleId = roleId });
    }

    public Task<ResultWrapper<int>> UpdateOwners(int ownerId, int[] users)
    {
        return _igRPCUserRepository.UpdateOwners(new UpdateOwnersRequest { OwnerId = ownerId, Users = users });
    }
}
