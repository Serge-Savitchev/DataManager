using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;

/// <summary>
/// Interface for UserRepository in gRPC server.
/// </summary>
[ServiceContract]
public interface IgRPCUsersRepository
{
    /// <summary>
    /// Deletes user by Id.
    /// </summary>
    /// <param name="userId"><see cref="Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<User>> DeleteUserAsync(Int32Request userId, CallContext context = default);

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    /// <param name="userId"><see cref="Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<User>> GetUserAsync(Int32Request userId, CallContext context = default);

    /// <summary>
    /// Gets users by role.
    /// </summary>
    /// <param name="roleId"><see cref="RoleRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleRequest roleId, CallContext context = default);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="empty"><see cref="StringRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<User[]>> GetAllUsersAsync(StringRequest empty, CallContext context = default);

    /// <summary>
    /// Updates owner of users.
    /// </summary>
    /// <param name="request"><see cref="UpdateOwnersRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<int>> UpdateOwnersAsync(UpdateOwnersRequest request, CallContext context = default);

}
