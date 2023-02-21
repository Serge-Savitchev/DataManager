using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

/// <summary>
/// Model of user credentials in database.
/// </summary>
[DataContract]
public class UserCredentials
{
    /// <summary>
    /// Unique Id of user
    /// </summary>
    [DataMember(Order = 1)]
    [Key]
    public int UserId { get; set; }

    /// <summary>
    /// User login
    /// </summary>
    [DataMember(Order = 2)]
    [Required]
    [StringLength(256)]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Password hash
    /// </summary>
    [DataMember(Order = 3)]
    [Required]
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Refresh token
    /// </summary>
    [DataMember(Order = 4)]
    public string? RefreshToken { get; set; } = null;
}
