﻿using System.ComponentModel.DataAnnotations;

namespace DataManagerAPI.Models;

public class UserData
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;
    public string? Data { get; set; }
}
