using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

/// <summary>
/// Request for updating users owner.
/// </summary>
public class UpdateOwnerRequestDto
{
    /// <summary>
    /// Owner Id.
    /// </summary>
    public int OwnerId { get; set; }

    /// <summary>
    /// Array of user Ids to be updated.
    /// </summary>
    [Required]
    [MinLength(1)]
    public int[] UserIds { get; set; } = Array.Empty<int>();
}
