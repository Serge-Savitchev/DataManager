﻿using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Dto;

/// <summary>
/// User data for frontend.
/// </summary>
public class AddUserDataDto
{
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
}
