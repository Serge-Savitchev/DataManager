using System.Text.Json.Serialization;

namespace DataManagerAPI.Models;

//[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleId : int
{
    Admin = 0,
    User = 1,
    AdvancedUser = 2,
    ReadOnlyUser = 3
}
