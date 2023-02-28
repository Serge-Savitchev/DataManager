using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManagerAPI.SQLServerDB.Implementation;

/// <summary>
/// Implementation of <see cref="IUsersRepository"/>.
/// </summary>
public class UsersRepository : IUsersRepository
{
    private readonly UsersDBContext _context;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context"><see cref="UsersDBContext"/></param>
    public UsersRepository(UsersDBContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User>> DeleteUserAsync(int userId,
            CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<User>
        {
            Success = true
        };

        try
        {
            var userToDelete = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (userToDelete is null)
            {
                result.Success = false;
                result.StatusCode = ResultStatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            result.Data = userToDelete;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = ResultStatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User>> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<User>
        {
            Success = true
        };

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user is null)
            {
                result.Success = false;
                result.StatusCode = ResultStatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }
            result.Data = user;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = ResultStatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleIds roleId,
            CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<User[]>
        {
            Success = true
        };

        try
        {
            var users = await _context.Users.Where(x => x.Role == roleId).ToArrayAsync(cancellationToken);
            result.Data = users;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = ResultStatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User[]>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<User[]>
        {
            Success = true
        };

        try
        {
            var users = await _context.Users.ToArrayAsync(cancellationToken);
            result.Data = users;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = ResultStatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> UpdateOwnersAsync(int ownerId, int[] users,
                CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<int>
        {
            Success = true
        };

        try
        {
            IEnumerable<User> res = from u in users
                                    join user in _context.Users
                                    on u equals user.Id
                                    select user;

            res.AsParallel().ForAll(x => x.OwnerId = ownerId);

            await _context.SaveChangesAsync(cancellationToken);
            result.Data = res.Count();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = ResultStatusCodes.Status500InternalServerError;
        }

        return result;
    }
}