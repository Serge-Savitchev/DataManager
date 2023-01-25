using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Services;

public class UserPasswordService : IUserPasswordService
{
    public UserCredentials CreatePasswordHash(string password)
    {
        var result = new UserCredentials();

        if (!string.IsNullOrWhiteSpace(password))
        {
            result.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        return result;
    }

    public bool VerifyPasswordHash(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
