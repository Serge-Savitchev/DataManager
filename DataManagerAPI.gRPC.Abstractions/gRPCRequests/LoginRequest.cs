using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class LoginRequest
{
    /// <summary>
    /// Login
    /// </summary>
    [DataMember(Order = 1)]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Credentials
    /// </summary>
    [DataMember(Order = 2)]
    public UserCredentials? Credentials { get; set; }
}
