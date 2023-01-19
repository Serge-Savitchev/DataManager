using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class UpdateUserRoleRequest
{
    [DataMember(Order = 1)]
    public int UserId { get; set; }

    [DataMember(Order = 2)]
    public RoleIds Role { get; set; }
}
