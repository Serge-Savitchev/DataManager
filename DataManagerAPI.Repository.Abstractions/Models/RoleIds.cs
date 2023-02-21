namespace DataManagerAPI.Repository.Abstractions.Models;

/// <summary>
/// User role Ids.
/// </summary>
public enum RoleIds : int
{
    /// <summary>
    /// Admin
    /// </summary>
    Admin = 1,

    /// <summary>
    /// PowerUser
    /// </summary>
    PowerUser = 2,

    /// <summary>
    /// User
    /// </summary>
    User = 3,

    /// <summary>
    /// ReadOnlyUser
    /// </summary>
    ReadOnlyUser = 4
}
