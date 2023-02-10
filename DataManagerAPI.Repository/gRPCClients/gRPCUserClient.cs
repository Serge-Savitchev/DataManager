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
    public Task<ResultWrapper<User>> DeleteUserAsync(int userId)
    {
        return _igRPCUserRepository.DeleteUserAsync(new UserIdRequest { UserId = userId });
    }

    public Task<ResultWrapper<List<User>>> GetAllUsersAsync()
    {
        return _igRPCUserRepository.GetAllUsersAsync(new EmptyRequest());
    }

    public Task<ResultWrapper<User>> GetUserAsync(int userId)
    {
        return _igRPCUserRepository.GetUserAsync(new UserIdRequest { UserId = userId });
    }

    public Task<ResultWrapper<List<User>>> GetUsersByRoleAsync(RoleIds roleId)
    {
        return _igRPCUserRepository.GetUsersByRoleAsync(new RoleRequest { RoleId = roleId });
    }

    public Task<ResultWrapper<int>> UpdateOwnersAsync(int ownerId, int[] users)
    {
        return _igRPCUserRepository.UpdateOwnersAsync(new UpdateOwnersRequest { OwnerId = ownerId, Users = users });
    }
}
