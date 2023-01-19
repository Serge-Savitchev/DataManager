using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class EmptyRequest
{
    [DataMember(Order = 1)]
    public string Empty { get; set; } = string.Empty;
}
