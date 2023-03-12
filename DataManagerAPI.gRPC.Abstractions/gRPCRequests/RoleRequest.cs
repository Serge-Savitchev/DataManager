using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class RoleRequest
{
    /// <summary>
    /// RoleId
    /// </summary>
    [DataMember(Order = 1)]
    public RoleIds RoleId { get; set; }
}
