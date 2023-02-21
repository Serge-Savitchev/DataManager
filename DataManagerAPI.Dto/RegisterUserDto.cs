using DataManagerAPI.Repository.Abstractions.Helpers;
using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

/// <summary>
/// Data for register of user.
/// </summary>
public class RegisterUserDto
{
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
    [RoleValidation]
    [StringLength(16)]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Login
    /// </summary>
    [Required]
    [StringLength(256)]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    [StringLength(256)]
    public string Password { get; set; } = string.Empty;
}
