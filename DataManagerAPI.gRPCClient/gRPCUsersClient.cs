using DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace DataManagerAPI.gRPCClient;

/// <summary>
/// Implementation of <see cref="IUsersRepository"/> for gRPC client.
/// </summary>
public class gRPCUsersClient : IUsersRepository
{
    private readonly IgRPCUsersRepository _igRPCUserRepository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="channel"><see cref="GrpcChannel"/></param>
    public gRPCUsersClient(GrpcChannel channel)
    {
        _igRPCUserRepository = channel.CreateGrpcService<IgRPCUsersRepository>();
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.DeleteUserAsync(new Int32Request { Value = userId },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User[]>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.GetAllUsersAsync(new StringRequest(),
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.GetUserAsync(new Int32Request { Value = userId },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleIds roleId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.GetUsersByRoleAsync(new RoleRequest { RoleId = roleId },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> UpdateOwnersAsync(int ownerId, int[] users, CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.UpdateOwnersAsync(new UpdateOwnersRequest { OwnerId = ownerId, Users = users },
            gRPCClientsHelper.CreateCallOptions(cancellationToken));
    }
}
