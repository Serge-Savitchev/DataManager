using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class UpdateUserRoleRequest
{
    /// <summary>
    /// UserId
    /// </summary>
    [DataMember(Order = 1)]
    public int UserId { get; set; }

    /// <summary>
    /// Role
    /// </summary>
    [DataMember(Order = 2)]
    public RoleIds Role { get; set; }
}
