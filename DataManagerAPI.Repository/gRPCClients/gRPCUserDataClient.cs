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
/// Implementation of <see cref="IUserDataRepository"/> for gRPC client.
/// </summary>
public class gRPCUserDataClient : IUserDataRepository
{
    private readonly IgRPCUserDataRepository _igRPCUserDataRepository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="channel"><see cref="GrpcChannel"/></param>
    public gRPCUserDataClient(GrpcChannel channel)
    {
        _igRPCUserDataRepository = channel.CreateGrpcService<IgRPCUserDataRepository>();
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd,
        CancellationToken cancellationToken = default)
    {
        return _igRPCUserDataRepository.AddUserDataAsync(userDataToAdd,
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> DeleteUserDataAsync(int userId, int userDataId,
        CancellationToken cancellationToken = default)
    {
        return _igRPCUserDataRepository.DeleteUserDataAsync(new Int32Int32Request { Value1 = userId, Value2 = userDataId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> GetUserDataAsync(int userId, int userDataId,
        CancellationToken cancellationToken = default)
    {
        return _igRPCUserDataRepository.GetUserDataAsync(new Int32Int32Request { Value1 = userId, Value2 = userDataId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        return _igRPCUserDataRepository.GetUserDataByUserIdAsync(new Int32Request { Value = userId },
            new CallOptions(cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate,
        CancellationToken cancellationToken = default)
    {
        return _igRPCUserDataRepository.UpdateUserDataAsync(userDataToUpdate,
            new CallOptions(cancellationToken: cancellationToken));
    }
}
