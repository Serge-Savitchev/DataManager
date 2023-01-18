using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

[DataContract]
public class UserCredentials
{
    [DataMember(Order = 1)]
    [Key]
    public int UserId { get; set; }

    [DataMember(Order = 2)]
    [Required]
    [StringLength(256)]
    public string Login { get; set; } = string.Empty;

    [DataMember(Order = 3)]
    [Required]
    public byte[] PasswordHash { get; set; } = null!;

    [DataMember(Order = 4)]
    [Required]
    public byte[] PasswordSalt { get; set; } = null!;

    [DataMember(Order = 5)]
    public string? RefreshToken { get; set; } = null;
}
