using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Models;

public class User
{
    public int Id { get; set; }
    [StringLength(64)]
    public string FirstName { get; set; } = string.Empty;
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;
    [StringLength(256)]
    public string? Email { get; set; }
    //public UserCredentials UserCredentials { get; set; } = new UserCredentials();
    public RoleId Role { get; set; }
}
