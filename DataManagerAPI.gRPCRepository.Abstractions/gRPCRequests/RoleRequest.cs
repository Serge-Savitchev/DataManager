using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class RoleRequest
{
    [DataMember(Order = 1)]
    public RoleIds RoleId { get; set; }
}
