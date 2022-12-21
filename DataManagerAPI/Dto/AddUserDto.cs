using DataManagerAPI.Helpers;
using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

public class AddUserDto
{
    [Required]
    [StringLength(64)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;
    [StringLength(256)]
    public string? Email { get; set; }
    [Required]
    [RoleValidation]
    [StringLength(16)]
    public string Role { get; set; } = string.Empty;
    [Required]
    [StringLength(256)]
    public string Login { get; set; } = string.Empty;
    [Required]
    [StringLength(256)]
    public string Password { get; set; } = string.Empty;
}
