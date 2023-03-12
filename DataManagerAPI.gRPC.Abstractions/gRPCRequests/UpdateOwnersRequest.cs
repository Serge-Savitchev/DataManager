using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class UpdateOwnersRequest
{
    /// <summary>
    /// OwnerId
    /// </summary>
    [DataMember(Order = 1)]
    public int OwnerId { get; set; }

    /// <summary>
    /// Users
    /// </summary>
    [DataMember(Order = 2)]
    public int[] Users { get; set; } = Array.Empty<int>();
}
