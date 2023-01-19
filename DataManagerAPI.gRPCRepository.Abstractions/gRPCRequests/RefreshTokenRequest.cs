using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class RefreshTokenRequest
{
    [DataMember(Order = 1)]
    public int UserId { get; set; }

    [DataMember(Order = 2)]
    public string? RefreshToken { get; set; }
}
