using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataManagerAPI.SQLServerDB.Implementation;

/// <summary>
/// Implementation of <see cref="IUsersRepository"/>.
/// </summary>
public class UsersRepository : IUsersRepository
{
    private readonly UsersDBContext _context;
    private readonly ILogger<UsersRepository> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context"><see cref="UsersDBContext"/></param>
    /// <param name="logger"></param>
    public UsersRepository(UsersDBContext context, ILogger<UsersRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User>> DeleteUserAsync(int userId,
            CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId}", userId);

        var result = new ResultWrapper<User>
        {
            Success = true
        };

        try
        {
            var userToDelete = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (userToDelete is null)
            {
                Helpers.LogNotFoundWarning(result, $"UserId {userId} not found", _logger);
                return result;
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            result.Data = userToDelete;
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User>> GetUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId}", userId);

        var result = new ResultWrapper<User>
        {
            Success = true
        };

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user is null)
            {
                Helpers.LogNotFoundWarning(result, $"UserId {userId} not found", _logger);
                return result;
            }
            result.Data = user;
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User[]>> GetUsersByRoleAsync(RoleIds roleId,
            CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{roleId}", roleId);

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
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User[]>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started");

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
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<int>> UpdateOwnersAsync(int ownerId, int[] users,
                CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{owerId}", ownerId);

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
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }
}