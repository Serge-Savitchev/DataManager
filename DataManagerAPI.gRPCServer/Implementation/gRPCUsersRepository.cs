using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

/// <summary>
/// Implementation of <see cref="IgRPCUsersRepository"/> for gRPC server.
/// </summary>
public class gRPCUsersRepository : IgRPCUsersRepository
{
    private readonly IUsersRepository _repository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUsersRepository"/></param>
    public gRPCUsersRepository(IUsersRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> DeleteUserAsync(Int32Request userId, CallContext context = default)
    {
        return _repository.DeleteUserAsync(userId.Value);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User[]>> GetAllUsersAsync(StringRequest empty, CallContext context = default)
    {
        return _repository.GetAllUsersAsync();
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> GetUserAsync(Int32Request userId, CallContext context = default)
    {
        return _repository.GetUserAsync(userId.Value);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleRequest roleId, CallContext context = default)
    {
        return _repository.GetUsersByRoleAsync(roleId.RoleId);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> UpdateOwnersAsync(UpdateOwnersRequest request, CallContext context = default)
    {
        return _repository.UpdateOwnersAsync(request.OwnerId, request.Users);
    }
}
