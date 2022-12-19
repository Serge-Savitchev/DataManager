using DataManagerAPI.Helpers;
using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto
{
    public class AddUserDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        [Required]
        [RoleValidation]
        public string Role { get; set; } = string.Empty;
        [Required]
        [StringLength(256)]
        public string Login { get; set; } = string.Empty;
        [Required]
        [StringLength(256)]
        public string Password { get; set; } = string.Empty;
    }
}
