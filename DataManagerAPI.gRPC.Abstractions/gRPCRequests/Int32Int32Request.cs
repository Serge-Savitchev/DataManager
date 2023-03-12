using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class Int32Int32Request
{
    /// <summary>
    /// Value1
    /// </summary>
    [DataMember(Order = 1)]
    public int Value1 { get; set; }

    /// <summary>
    /// Value2
    /// </summary>
    [DataMember(Order = 2)]
    public int Value2 { get; set; }
}
