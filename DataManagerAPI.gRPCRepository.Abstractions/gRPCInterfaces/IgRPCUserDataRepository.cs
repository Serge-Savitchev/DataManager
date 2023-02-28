using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;

/// <summary>
/// Interface for UserDataRepository in gRPC server.
/// It provides access to database via gRPC service. It has the same set of methods as IUserDataRepository interface has.
/// </summary>
[ServiceContract]
public interface IgRPCUserDataRepository
{
    /// <summary>
    /// Adds new User Data.
    /// </summary>
    /// <param name="userDataToAdd"><see cref="UserData"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd, CallContext context = default);

    /// <summary>
    /// Updates User Data.
    /// </summary>
    /// <param name="userDataToUpdate"><see cref="UserData"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate, CallContext context = default);

    /// <summary>
    /// Deletes User Data by Id.
    /// </summary>
    /// <param name="userDataId"><see cref="Int32Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<UserData>> DeleteUserDataAsync(Int32Int32Request userDataId, CallContext context = default);

    /// <summary>
    /// Gets User Data by Id.
    /// </summary>
    /// <param name="userDataId"><see cref="Int32Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<UserData>> GetUserDataAsync(Int32Int32Request userDataId, CallContext context = default);

    /// <summary>
    /// Gets all User Data by user Id.
    /// </summary>
    /// <param name="userId"><see cref="Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns></returns>
    [OperationContract]
    Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(Int32Request userId, CallContext context = default);

    /// <summary>
    /// Returns owning user of UserData by UserData Id.
    /// </summary>
    /// <param name="userDataId">Id of user data<see cref="Int32Request"/></param>
    /// <param name="context"><see cref="CallContext"/></param>
    /// <returns><see cref="User"/></returns>
    [OperationContract]
    Task<ResultWrapper<User>> GetUserAsync(Int32Request userDataId, CallContext context = default);
}
