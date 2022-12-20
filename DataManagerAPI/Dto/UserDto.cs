using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

public class UserDto
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    [Required]
    public string Role { get; set; } = string.Empty;
}
