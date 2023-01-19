using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class UserIdRequest
{
    [DataMember(Order = 1)]
    public int UserId { get; set; }
}
