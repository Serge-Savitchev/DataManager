namespace DataManagerAPI.Services;

/// <summary>
/// Interface for support of logged out users.
/// </summary>
public interface ILoggedOutUsersCollectionService
{
    /// <summary>
    /// Add user to list of logged out users.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>True if success</returns>
    bool Add(int userId);

    /// <summary>
    /// Remove user from list of logged out users.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <returns>True if success</returns>
    bool Remove(int userId);

    /// <summary>
    /// Check if user is in the list of logged out users.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>True if success</returns>
    bool Contains(int userId);
}
