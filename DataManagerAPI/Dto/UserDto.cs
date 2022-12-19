using DataManagerAPI.Models;

namespace DataManagerAPI.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Role { get; set; } = string.Empty;
        //public List<UserDataDto> UserData { get; set; } = new List<UserDataDto>();

    }
}
