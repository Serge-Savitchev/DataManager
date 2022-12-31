using DataManagerAPI.Dto;

namespace DataManagerAPI.Integration.Tests
{
    public class RegisterUserTestData
    {
        public bool Locked { get; set; } = false;
        public int Id { get; set; } = -1;
        public RegisterUserDto UserData { get; set; } = new RegisterUserDto();
        public LoginUserResponseDto? LoginData { get; set; }
    }
}
