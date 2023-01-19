using DataManagerAPI.Repository.Abstractions.Models;
using System.Runtime.Serialization;

namespace DataManagerAPI.gRPCRepository.Abstractions.gRPCRequests;

[DataContract]
public class LoginRequest
{
    [DataMember(Order = 1)]
    public string Login { get; set; } = string.Empty;

    [DataMember(Order = 2)]
    public UserCredentials? Credentials { get; set; }
}
