using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

[DataContract]
public class UserData
{
    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public int UserId { get; set; }

    [DataMember(Order = 3)]
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = string.Empty;

    [DataMember(Order = 4)]
    public string? Data { get; set; }
}
