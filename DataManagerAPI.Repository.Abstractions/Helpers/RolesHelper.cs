using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Repository.Abstractions.Helpers;

public static class RolesHelper
{
    public static string[] GetAllNames()
    {
        var roles = new List<string>();
        foreach (RoleIds s in Enum.GetValues(typeof(RoleIds)))
        {
            roles.Add(s.ToString());
        }

        return roles.ToArray();
    }
}
