using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

/// <summary>
/// Description of file in database.
/// </summary>
[DataContract]
public class UserFile
{
    /// <summary>
    /// Key.
    /// </summary>
    [Key]
    [DataMember(Order = 1)]
    public int Id { get; set; }

    /// <summary>
    /// Id of User Data
    /// </summary>
    [DataMember(Order = 2)]
    public int UserDataId { get; set; }

    /// <summary>
    /// File name.
    /// </summary>
    [DataMember(Order = 3)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Size of file in bytes.
    /// </summary>
    [DataMember(Order = 4)]
    public long Size { get; set; }
}
