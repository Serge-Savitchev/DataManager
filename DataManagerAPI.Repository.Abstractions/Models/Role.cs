using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

/// <summary>
/// Model of user role in database.
/// </summary>
[DataContract]
public class Role
{
    /// <summary>
    /// Role Id. <see cref="RoleIds"/>.
    /// </summary>
    [DataMember(Order = 1)]
    public RoleIds Id { get; set; } = RoleIds.Admin;

    /// <summary>
    /// Role name.
    /// </summary>
    [DataMember(Order = 2)]
    public string Name { get; set; } = Enum.GetName(typeof(RoleIds), RoleIds.Admin)!;
}
