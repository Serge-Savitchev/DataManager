using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

public class UserDto
{
    public int Id { get; set; }
    [Required]
    [StringLength(64)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;
    [StringLength(256)]
    public string? Email { get; set; }
    [Required]
    [StringLength(16)]
    public string Role { get; set; } = string.Empty;
    public int OwnerId { get; set; }
}
