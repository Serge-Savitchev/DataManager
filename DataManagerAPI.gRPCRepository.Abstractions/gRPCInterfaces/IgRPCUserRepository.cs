using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;

[ServiceContract]
public interface IgRPCUserRepository
{
    [OperationContract]
    Task<ResultWrapper<User>> DeleteUser(UserIdRequest userId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<User>> GetUser(UserIdRequest userId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<List<User>>> GetUsersByRole(RoleRequest roleId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<List<User>>> GetAllUsers(EmptyRequest empty, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<int>> UpdateOwners(UpdateOwnersRequest request, CallContext context = default);

}
