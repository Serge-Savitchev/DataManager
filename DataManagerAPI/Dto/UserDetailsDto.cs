using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto
{
    public class UserDetailsDto : UserDto
    {
        [StringLength(256)]
        public string Login { get; set; } = string.Empty;
    }
}
