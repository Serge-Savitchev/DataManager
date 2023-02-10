using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;

[ServiceContract]
public interface IgRPCUserDataRepository
{
    [OperationContract]
    Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<UserData>> DeleteUserDataAsync(UserIdRequest userDataId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<UserData>> GetUserDataAsync(UserIdRequest userDataId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(UserIdRequest userId, CallContext context = default);

}
