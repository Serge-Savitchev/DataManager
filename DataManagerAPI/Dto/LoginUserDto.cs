using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto
{
    public class LoginUserDto
    {
        [Required]
        [StringLength(256)]
        public string Login { get; set; } = string.Empty;
        [Required]
        [StringLength(256)]
        public string Password { get; set; } = string.Empty;
    }
}
