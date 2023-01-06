using System.Text.Json.Serialization;

namespace DataManagerAPI.Repository.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public class Role
{
    public RoleIds Id { get; set; } = RoleIds.Admin;
    public string Name { get; set; } = Enum.GetName(typeof(RoleIds), RoleIds.Admin)!;
}
