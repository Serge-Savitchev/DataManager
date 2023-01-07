using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Repository.Abstractions.Models;

public class UserCredentials
{
    [Key]
    public int UserId { get; set; }
    [Required]
    [StringLength(256)]
    public string Login { get; set; } = string.Empty;
    [Required]
    public byte[] PasswordHash { get; set; } = null!;
    [Required]
    public byte[] PasswordSalt { get; set; } = null!;
    public string? RefreshToken { get; set; } = null;
}
