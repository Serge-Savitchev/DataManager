using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Models;

public class UserCredentials
{
    [Required]
    [StringLength(256)]
    public string Login { get; set; } = string.Empty;
    [Required]
    public byte[] PasswordHash { get; set; } = null!;
    [Required]
    public byte[] PasswordSalt { get; set; } = null!;
}
