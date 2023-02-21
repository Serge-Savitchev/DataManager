using DataManagerAPI.Repository.Abstractions.Models;
using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

/// <summary>
/// UserData for forntend.
/// </summary>
public class UserDataDto
{
    /// <summary>
    /// Unique Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User Id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Data title
    /// </summary>
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Data body
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// List of attached files
    /// </summary>
    public UserFile[] UserFiles { get; set; } = Array.Empty<UserFile>();
}
