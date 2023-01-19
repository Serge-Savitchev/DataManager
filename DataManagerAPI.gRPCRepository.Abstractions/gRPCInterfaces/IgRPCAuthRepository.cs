using DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Models;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCInterfaces;

[ServiceContract]
public interface IgRPCAuthRepository
{
    [OperationContract]
    Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(UserIdRequest request, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(LoginValueRequest login, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<int>> LoginAsync(LoginRequest request, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<int>> RefreshTokenAsync(RefreshTokenRequest request, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<User>> RegisterUserAsync(RegisterUserRequest request, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<int>> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, CallContext context = default);

    [OperationContract]
    Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(UpdateUserRoleRequest request, CallContext context = default);
}
