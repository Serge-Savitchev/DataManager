using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Grpc.Core;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace DataManagerAPI.Repository.gRPCClients;

/// <summary>
/// Implementation of <see cref="IUserRepository"/> for gRPC client.
/// </summary>
public class gRPCUserClient : IUserRepository
{
    private readonly IgRPCUsersRepository _igRPCUserRepository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="channel"><see cref="GrpcChannel"/></param>
    public gRPCUserClient(GrpcChannel channel)
    {
        _igRPCUserRepository = channel.CreateGrpcService<IgRPCUsersRepository>();
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.DeleteUserAsync(new Int32Request { Value = userId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User[]>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.GetAllUsersAsync(new StringRequest(),
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User>> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.GetUserAsync(new Int32Request { Value = userId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleIds roleId, CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.GetUsersByRoleAsync(new RoleRequest { RoleId = roleId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<int>> UpdateOwnersAsync(int ownerId, int[] users, CancellationToken cancellationToken = default)
    {
        return _igRPCUserRepository.UpdateOwnersAsync(new UpdateOwnersRequest { OwnerId = ownerId, Users = users },
            new CallOptions(cancellationToken: cancellationToken));
    }
}
