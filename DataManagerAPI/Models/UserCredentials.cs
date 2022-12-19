namespace DataManagerAPI.Models;

public class UserCredentials
{
    public int UserId { get; set; }
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
}
