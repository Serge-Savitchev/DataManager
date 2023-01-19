using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

public class gRPCUserRepository : IgRPCUserRepository
{
    private readonly IUserRepository _repository;

    public gRPCUserRepository(IUserRepository repository)
    {
        _repository = repository;
    }

    public Task<ResultWrapper<User>> DeleteUser(UserIdRequest userId, CallContext context = default)
    {
        return _repository.DeleteUser(userId.UserId);
    }

    public Task<ResultWrapper<List<User>>> GetAllUsers(EmptyRequest empty, CallContext context = default)
    {
        return _repository.GetAllUsers();
    }

    public Task<ResultWrapper<User>> GetUser(UserIdRequest userId, CallContext context = default)
    {
        return _repository.GetUser(userId.UserId);
    }

    public Task<ResultWrapper<List<User>>> GetUsersByRole(RoleRequest roleId, CallContext context = default)
    {
        return _repository.GetUsersByRole(roleId.RoleId);
    }

    public Task<ResultWrapper<int>> UpdateOwners(UpdateOwnersRequest request, CallContext context = default)
    {
        return _repository.UpdateOwners(request.OwnerId, request.Users);
    }
}
