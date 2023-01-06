using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Repository.Models;

public class User
{
    public int Id { get; set; }
    [StringLength(64)]
    public string FirstName { get; set; } = string.Empty;
    [StringLength(64)]
    public string LastName { get; set; } = string.Empty;
    [StringLength(256)]
    public string? Email { get; set; }
    public RoleIds Role { get; set; }
    public int OwnerId { get; set; }
}
