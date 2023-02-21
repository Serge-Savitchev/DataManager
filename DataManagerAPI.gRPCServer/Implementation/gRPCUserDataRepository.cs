using DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;
using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;

namespace DataManagerAPI.gRPCServer.Implementation;

/// <summary>
/// Implementation of <see cref="IgRPCUserDataRepository"/> for gRPC server.
/// </summary>
public class gRPCUserDataRepository : IgRPCUserDataRepository
{
    private readonly IUserDataRepository _repository;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="repository"><see cref="IUserDataRepository"/></param>
    public gRPCUserDataRepository(IUserDataRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd, CallContext context = default)
    {
        return _repository.AddUserDataAsync(userDataToAdd);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> DeleteUserDataAsync(Int32Request userDataId, CallContext context = default)
    {
        return _repository.DeleteUserDataAsync(userDataId.Value);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> GetUserDataAsync(Int32Request userDataId, CallContext context = default)
    {
        return _repository.GetUserDataAsync(userDataId.Value);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(Int32Request userId, CallContext context = default)
    {
        return _repository.GetUserDataByUserIdAsync(userId.Value);
    }

    /// <inheritdoc />
    public Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate, CallContext context = default)
    {
        return _repository.UpdateUserDataAsync(userDataToUpdate);
    }
}
