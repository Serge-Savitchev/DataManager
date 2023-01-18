using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

[DataContract]
public class UserCredentialsData
{
    [DataMember(Order = 1)]
    public User? User { get; set; } = null;

    [DataMember(Order = 2)]
    public UserCredentials? Credentials { get; set; } = null;
}
