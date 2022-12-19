using System.Text.Json.Serialization;

namespace DataManagerAPI.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public class Role
{
    public RoleId Id { get; set; } = RoleId.Admin;
    public string Name { get; set; } = Enum.GetName(typeof(RoleId), RoleId.Admin)!;
}
