using DataManagerAPI.Dto;

namespace DataManagerAPI.Tests.IntegrationTests;

public class RegisteredUserTestData : IDisposable
{
    public bool Locked { get; set; } = false;
    public int Id { get; set; } = -1;
    public RegisteredUserDto RegisteredUser { get; set; } = new RegisteredUserDto();
    public LoginUserResponseDto? LoginData { get; set; }

    public void Dispose()
    {
        Locked = false;
        LoginData = null;
    }
}
