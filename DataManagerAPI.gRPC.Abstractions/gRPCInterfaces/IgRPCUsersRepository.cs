using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;

/// <summary>
/// Interface for UserRepository in gRPC server.
/// It provides access to database via gRPC service and has the same set of methods as IUsersRepository has.
/// </summary>
[ServiceContract]
public interface IgRPCUsersRepository
{
    /// <summary>
    /// Deletes user by Id.
    /// </summary>
    /// <param name="userId"><see cref="Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>Deleted user. <see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User>> DeleteUserAsync(Int32Request userId, CallContext context = default);

    /// <summary>
    /// Gets user by Id.
    /// </summary>
    /// <param name="userId"><see cref="Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns><see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User>> GetUserAsync(Int32Request userId, CallContext context = default);

    /// <summary>
    /// Gets users by role.
    /// </summary>
    /// <param name="roleId"><see cref="RoleRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>Array of found users. <see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleRequest roleId, CallContext context = default);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="empty"><see cref="StringRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>Array of all users. <see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User[]>> GetAllUsersAsync(StringRequest empty, CallContext context = default);

    /// <summary>
    /// Updates owner of users.
    /// </summary>
    /// <param name="request"><see cref="UpdateOwnersRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>Count of users with changed owner</returns>
    [OperationContract]
    Task<ResultWrapper<int>> UpdateOwnersAsync(UpdateOwnersRequest request, CallContext context = default);

}
