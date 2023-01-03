using DataManagerAPI.Dto;

namespace DataManagerAPI.Tests.IntegrationTests
{
    public class RegisterUserTestData : IDisposable
    {
        public bool Locked { get; set; } = false;
        public int Id { get; set; } = -1;
        public RegisterUserDto UserData { get; set; } = new RegisterUserDto();
        public LoginUserResponseDto? LoginData { get; set; }

        public void Dispose()
        {
            Locked = false;
            LoginData = null;
        }
    }
}
