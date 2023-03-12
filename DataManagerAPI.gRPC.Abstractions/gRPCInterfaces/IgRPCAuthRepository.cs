using DataManagerAPI.gRPC.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace DataManagerAPI.gRPC.Abstractions.gRPCInterfaces;

/// <summary>
/// Interface for AuthRepository in gRPC server.
/// It provides access to database via gRPC service. It has the same set of methods as IAuthRepository interface has.
/// </summary>
[ServiceContract]
public interface IgRPCAuthRepository
{
    /// <summary>
    /// Get user's credentials.
    /// </summary>
    /// <param name="request"><see cref="Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns><see cref="UserCredentialsData"/></returns>
    [OperationContract]
    Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(Int32Request request, CallContext context = default);

    /// <summary>
    /// Searches user by login.
    /// </summary>
    /// <param name="login"><see cref="StringRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns><see cref="UserCredentialsData"/></returns>
    [OperationContract]
    Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(StringRequest login, CallContext context = default);

    /// <summary>
    /// Authenticates and authorizes user.
    /// </summary>
    /// <param name="request"><see cref="LoginRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>>User Id</returns>
    [OperationContract]
    Task<ResultWrapper<int>> LoginAsync(LoginRequest request, CallContext context = default);

    /// <summary>
    /// Stores new Refresh token.
    /// </summary>
    /// <param name="request"><see cref="RefreshTokenRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>User Id</returns>
    [OperationContract]
    Task<ResultWrapper<int>> RefreshTokenAsync(RefreshTokenRequest request, CallContext context = default);

    /// <summary>
    /// Register new user.
    /// </summary>
    /// <param name="request"><see cref="RegisterUserRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns><see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User>> RegisterUserAsync(RegisterUserRequest request, CallContext context = default);

    /// <summary>
    /// Updates user password.
    /// </summary>
    /// <param name="request"><see cref="UpdateUserPasswordRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns>User Id</returns>
    [OperationContract]
    Task<ResultWrapper<int>> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CallContext context = default);

    /// <summary>
    /// Updates user role.
    /// </summary>
    /// <param name="request"><see cref="UpdateUserRoleRequest"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(UpdateUserRoleRequest request, CallContext context = default);
}
