namespace DataManagerAPI.Dto;

public class LoginUserResponseDto : UserDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
