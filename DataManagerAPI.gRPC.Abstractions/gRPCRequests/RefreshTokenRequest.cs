using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class RefreshTokenRequest
{
    /// <summary>
    /// UserId
    /// </summary>
    [DataMember(Order = 1)]
    public int UserId { get; set; }

    /// <summary>
    /// Refresh token
    /// </summary>
    [DataMember(Order = 2)]
    public string? RefreshToken { get; set; }
}
