using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

/// <summary>
/// Model of User in database.
/// </summary>
[DataContract]
public class User
{
    /// <summary>
    /// Unique Id
    /// </summary>
    [DataMember(Order = 1)]
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    [DataMember(Order = 2)]
    [StringLength(64)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [DataMember(Order = 3)]
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    [DataMember(Order = 4)]
    [StringLength(256)]
    public string? Email { get; set; }

    /// <summary>
    /// Role Id
    /// </summary>
    [DataMember(Order = 5)]
    public RoleIds Role { get; set; }

    /// <summary>
    /// Owner Id
    /// </summary>
    [DataMember(Order = 6)]
    public int OwnerId { get; set; }
}
