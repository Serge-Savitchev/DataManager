using DataManagerAPI.Dto.Constants;

namespace DataManagerAPI.Dto.Helpers;

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
        foreach (RoleIdsDto s in Enum.GetValues(typeof(RoleIdsDto)))
        {
            roles.Add(s.ToString());
        }

        return roles.ToArray();
    }
}
