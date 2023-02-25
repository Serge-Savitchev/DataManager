using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Services.Interfaces;

/// <summary>
/// Interface for user credentials mnagement.
/// </summary>
public interface IUserPasswordService
{
    /// <summary>
    /// Create credentials for user. 
    /// </summary>
    /// <param name="password"><see cref="string"/></param>
    /// <returns>User credentials. <see cref="UserCredentials"/></returns>
    UserCredentials CreatePasswordHash(string password);

    /// <summary>
    /// Verifies user password.
    /// </summary>
    /// <param name="password"><see cref="string"/></param>
    /// <param name="passwordHash"><see cref="string"/></param>
    /// <returns>True if verification success</returns>
    bool VerifyPasswordHash(string password, string passwordHash);
}
