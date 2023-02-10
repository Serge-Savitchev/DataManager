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
    Task<ResultWrapper<User>> DeleteUserAsync(UserIdRequest userId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<User>> GetUserAsync(UserIdRequest userId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<List<User>>> GetUsersByRoleAsync(RoleRequest roleId, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<List<User>>> GetAllUsersAsync(EmptyRequest empty, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<int>> UpdateOwnersAsync(UpdateOwnersRequest request, CallContext context = default);

}
