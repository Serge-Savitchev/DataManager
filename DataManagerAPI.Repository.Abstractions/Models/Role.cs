using System.Runtime.Serialization;

namespace DataManagerAPI.Repository.Abstractions.Models;

[DataContract]
public class Role
{
    [DataMember(Order = 1)]
    public RoleIds Id { get; set; } = RoleIds.Admin;

    [DataMember(Order = 2)]
    public string Name { get; set; } = Enum.GetName(typeof(RoleIds), RoleIds.Admin)!;
}
