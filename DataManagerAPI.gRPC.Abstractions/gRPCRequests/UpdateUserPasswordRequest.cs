using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;

/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class UpdateUserPasswordRequest
{
    /// <summary>
    /// UserId
    /// </summary>
    [DataMember(Order = 1)]
    public int UserId { get; set; }

    /// <summary>
    /// UserCredentials
    /// </summary>
    [DataMember(Order = 2)]
    public UserCredentials? UserCredentials { get; set; }

}
