using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

/// <summary>
/// Model of user data in database.
/// </summary>
[DataContract]
public class UserData
{
    /// <summary>
    /// Unique Id
    /// </summary>
    [DataMember(Order = 1)]
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// User Id
    /// </summary>
    [DataMember(Order = 2)]
    public int UserId { get; set; }

    /// <summary>
    /// Data title
    /// </summary>
    [DataMember(Order = 3)]
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Data boby
    /// </summary>
    [DataMember(Order = 4)]
    public string? Data { get; set; }

    /// <summary>
    /// List of attached files
    /// </summary>
    [DataMember(Order = 5)]
    public List<UserFile> UserFiles { get; set; } = new List<UserFile>();
}
