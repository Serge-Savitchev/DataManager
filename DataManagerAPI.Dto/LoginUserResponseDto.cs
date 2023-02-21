namespace DataManagerAPI.Dto;

/// <summary>
/// Response from user's login.
/// </summary>
public class LoginUserResponseDto : UserDto
{
    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
