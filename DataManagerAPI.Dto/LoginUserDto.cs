using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

/// <summary>
/// Data passed from client during login.
/// </summary>
public class LoginUserDto
{
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
