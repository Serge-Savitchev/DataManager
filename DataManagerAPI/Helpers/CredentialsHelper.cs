using DataManagerAPI.Models;
using System.Security.Cryptography;

namespace DataManagerAPI.Helpers;

public static class CredentialsHelper
{
    public static UserCredentials? CreatePasswordHash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        using var hmac = new HMACSHA512();
        var result = new UserCredentials
        {
            PasswordSalt = hmac.Key,
            PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
        };

        return result;
    }
}
