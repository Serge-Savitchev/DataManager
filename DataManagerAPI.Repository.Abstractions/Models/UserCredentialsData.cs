using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;
/// <summary>
/// User credentials.
/// </summary>
[DataContract]
public class UserCredentialsData
{
    /// <summary>
    /// User
    /// </summary>
    [DataMember(Order = 1)]
    public User? User { get; set; } = null;

    /// <summary>
    /// User credentials
    /// </summary>
    [DataMember(Order = 2)]
    public UserCredentials? Credentials { get; set; } = null;
}
