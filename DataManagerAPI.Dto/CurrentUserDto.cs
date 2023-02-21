namespace DataManagerAPI.Dto;

/// <summary>
/// Current user crated from access token.
/// </summary>
public class CurrentUserDto
{
    /// <summary>
    /// User
    /// </summary>
    public UserDto? User { get; set; } = null;

    /// <summary>
    /// Login
    /// </summary>
    public string? Login { get; set; } = null;
}
