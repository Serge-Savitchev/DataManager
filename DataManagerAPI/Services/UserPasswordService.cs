﻿using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services.Interfaces;

namespace DataManagerAPI.Services;

/// <summary>
/// Implementation of <see cref="IUserPasswordService"/>.
/// </summary>
public class UserPasswordService : IUserPasswordService
{
    /// <inheritdoc />
    public UserCredentials CreatePasswordHash(string password)
    {
        var result = new UserCredentials();

        if (!string.IsNullOrWhiteSpace(password))
        {
            result.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        return result;
    }

    /// <inheritdoc />
    public bool VerifyPasswordHash(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
