using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

[DataContract]
public class User
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    [StringLength(64)]
    public string FirstName { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    [StringLength(256)]
    public string? Email { get; set; }

    [DataMember(Order = 5)]
    public RoleIds Role { get; set; }

    [DataMember(Order = 6)]
    public int OwnerId { get; set; }
}
