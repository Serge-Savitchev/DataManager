using System.ComponentModel.DataAnnotations.Schema;

namespace DataManagerAPI.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public UserCredentials UserCredentials { get; set; } = new UserCredentials();
    public RoleId Role { get; set; }
    public List<UserData> UserData { get; set; } = new List<UserData>();
}
