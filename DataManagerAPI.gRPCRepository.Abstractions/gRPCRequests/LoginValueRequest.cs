using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class LoginValueRequest
{
    [DataMember(Order = 1)]
    public string Login { get; set; } = string.Empty;
}
