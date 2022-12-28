namespace DataManagerAPI.Dto;

public class LoginUserResponseDto : UserDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
