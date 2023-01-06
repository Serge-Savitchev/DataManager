using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto
{
    public class UpdateOwnerRequest
    {
        public int OwnerId { get; set; }
        [Required]
        [MinLength(1)]
        public int[] UserIds { get; set; } = Array.Empty<int>();
    }
}
