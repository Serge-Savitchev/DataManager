using DataManagerAPI.Dto;

namespace DataManagerAPI.Tests.IntegrationTests;

public class RegisteredUserTestData : IDisposable
{
    public bool Locked { get; set; } = false;
    public int Id { get; set; } = -1;
    public RegisterUserDto RegisterUser { get; set; } = new RegisterUserDto();
    public LoginUserResponseDto? LoginData { get; set; }

    public void Dispose()
    {
        Locked = false;
        LoginData = null;
    }
}
