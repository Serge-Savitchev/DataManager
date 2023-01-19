using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class UpdateOwnersRequest
{
    [DataMember(Order = 1)]
    public int OwnerId { get; set; }

    [DataMember(Order = 2)]
    public int[] Users { get; set; } = Array.Empty<int>();
}
