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

    public Task<ResultWrapper<User>> DeleteUserAsync(UserIdRequest userId, CallContext context = default)
    {
        return _repository.DeleteUserAsync(userId.UserId);
    }

    public Task<ResultWrapper<List<User>>> GetAllUsersAsync(EmptyRequest empty, CallContext context = default)
    {
        return _repository.GetAllUsersAsync();
    }

    public Task<ResultWrapper<User>> GetUserAsync(UserIdRequest userId, CallContext context = default)
    {
        return _repository.GetUserAsync(userId.UserId);
    }

    public Task<ResultWrapper<List<User>>> GetUsersByRoleAsync(RoleRequest roleId, CallContext context = default)
    {
        return _repository.GetUsersByRoleAsync(roleId.RoleId);
    }

    public Task<ResultWrapper<int>> UpdateOwnersAsync(UpdateOwnersRequest request, CallContext context = default)
    {
        return _repository.UpdateOwnersAsync(request.OwnerId, request.Users);
    }
}
