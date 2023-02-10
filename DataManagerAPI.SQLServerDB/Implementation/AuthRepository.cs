using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataManagerAPI.SQLServerDB.Implementation;

/// <summary>
/// Implementation of <see cref="IAuthRepository"/>
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly UsersDBContext _context;

    public AuthRepository(UsersDBContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User>> RegisterUserAsync(User userToAdd, UserCredentials userCredentials)
    {
        var result = new ResultWrapper<User>
        {
            StatusCode = StatusCodes.Status201Created,
            Success = true
        };

        try
        {
            var existingCredentials = await _context.UserCredentials
                .FirstOrDefaultAsync(x => x.Login == userCredentials.Login);

            if (existingCredentials != null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status409Conflict;
                result.Message = $"User with login {userCredentials.Login} already exists.";
                return result;
            }

            await _context.Users.AddAsync(userToAdd);
            await _context.SaveChangesAsync();

            userCredentials.UserId = userToAdd.Id;
            await _context.UserCredentials.AddAsync(userCredentials);
            await _context.SaveChangesAsync();

            result.Data = userToAdd;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> LoginAsync(string login, UserCredentials credentials)
    {
        var result = new ResultWrapper<int>
        {
            Success = true
        };

        try
        {
            var userCredentials = await _context.UserCredentials
                .FirstOrDefaultAsync(x => x.Login == login);

            if (userCredentials is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"User with login {login} not found.";

                return result;
            }

            userCredentials.RefreshToken = credentials.RefreshToken;

            await _context.SaveChangesAsync();
            result.Data = userCredentials.UserId;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> RefreshTokenAsync(int userId, string? refreshToken)
    {
        var result = new ResultWrapper<int>
        {
            Success = true
        };

        try
        {
            var userCredentials = await _context.UserCredentials
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (userCredentials is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"User with Id {userId} not found.";

                return result;
            }

            userCredentials.RefreshToken = refreshToken;

            await _context.SaveChangesAsync();
            result.Data = userCredentials.UserId;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByIdAsync(int userId)
    {
        var result = new ResultWrapper<UserCredentialsData>
        {
            Success = true
        };

        try
        {
            var res = from User in _context.Users
                      where User.Id == userId
                      join Credentials in _context.UserCredentials
                      on User.Id equals Credentials.UserId
                      select new UserCredentialsData { User = User, Credentials = Credentials };

            var data = await res.FirstOrDefaultAsync();

            if (data is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }

            result.Data = data;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserCredentialsData>> GetUserDetailsByLoginAsync(string login)
    {
        var result = new ResultWrapper<UserCredentialsData>
        {
            Success = true
        };

        try
        {
            var res = from Credentials in _context.UserCredentials
                      where Credentials.Login == login
                      join User in _context.Users
                      on Credentials.UserId equals User.Id
                      select new UserCredentialsData { User = User, Credentials = Credentials };

            var data = await res.FirstOrDefaultAsync();

            if (data is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"Login {login} not found";

                return result;
            }

            result.Data = data;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> UpdateUserPasswordAsync(int userId, UserCredentials credentials)
    {
        var result = new ResultWrapper<int>
        {
            Success = true
        };

        try
        {
            var updatedCredentials = await _context.UserCredentials.FirstOrDefaultAsync(x => x.UserId == userId);
            if (updatedCredentials is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }

            updatedCredentials.PasswordHash = credentials.PasswordHash;

            await _context.SaveChangesAsync();

            result.Data = userId;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<RoleIds>> UpdateUserRoleAsync(int userId, RoleIds newRole)
    {
        var result = new ResultWrapper<RoleIds>
        {
            Success = true
        };

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }

            if (user.Role != newRole)
            {
                user.Role = newRole;
                _context.SaveChanges();
            }
            else
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status409Conflict;
                result.Message = "The user role has not been chanhed.";
            }

            result.Data = user.Role;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }
}
