using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPC.Abstractions.gRPCRequests;


/// <summary>
/// gRPC request.
/// </summary>
[DataContract]
public class RegisterUserRequest
{
    /// <summary>
    /// User
    /// </summary>
    [DataMember(Order = 1)]
    public User? User { get; set; }

    /// <summary>
    /// UserCredentials
    /// </summary>
    [DataMember(Order = 2)]
    public UserCredentials? UserCredentials { get; set; }
}
