using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

/// <summary>
/// Pair of tokens.
/// </summary>
public class TokenApiModelDto
{
    /// <summary>
    /// Access token.
    /// </summary>
    [Required]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token.
    /// </summary>
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
