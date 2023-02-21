using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

/// <summary>
/// User for frontend
/// </summary>
public class UserDto
{
    /// <summary>
    /// User Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    [Required]
    [StringLength(64)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [Required]
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    [StringLength(256)]
    public string? Email { get; set; }

    /// <summary>
    /// Role name
    /// </summary>
    [Required]
    [StringLength(16)]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Owner Id
    /// </summary>
    public int OwnerId { get; set; }
}
