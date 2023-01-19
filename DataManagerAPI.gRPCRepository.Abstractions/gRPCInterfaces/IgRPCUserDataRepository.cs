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
    Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<UserData>> UpdateUserData(UserData userDataToUpdate, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<UserData>> DeleteUserData(UserIdRequest userDataId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<UserData>> GetUserData(UserIdRequest userDataId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<List<UserData>>> GetUserDataByUserId(UserIdRequest userId, CallContext context = default);

}
