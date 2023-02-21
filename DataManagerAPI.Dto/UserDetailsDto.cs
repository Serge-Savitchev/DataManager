using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

/// <summary>
/// User details for frontend
/// </summary>
public class UserDetailsDto : UserDto
{
    /// <summary>
    /// Login
    /// </summary>
    [StringLength(256)]
    public string Login { get; set; } = string.Empty;
}
