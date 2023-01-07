using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

public class TokenApiModelDto
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
