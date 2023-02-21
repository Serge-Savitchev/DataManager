using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Repository.Abstractions.Helpers;

/// <summary>
/// Helper for getting all user role names.
/// </summary>
public static class RolesHelper
{
    /// <summary>
    /// Returns list of names of all possible user roles.
    /// </summary>
    /// <returns></returns>
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
