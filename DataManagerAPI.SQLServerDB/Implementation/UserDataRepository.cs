using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataManagerAPI.SQLServerDB.Implementation;

/// <summary>
/// Implementation of <see cref="IUserDataRepository"/>.
/// </summary>
public class UserDataRepository : IUserDataRepository
{
    private readonly UsersDBContext _context;
    private readonly ILogger<UserDataRepository> _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context"><see cref="UsersDBContext"/></param>
    /// <param name="logger"></param>
    public UserDataRepository(UsersDBContext context, ILogger<UserDataRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId}", userDataToAdd.UserId);

        var result = new ResultWrapper<UserData>
        {
            StatusCode = ResultStatusCodes.Status201Created,
            Success = true
        };

        try
        {
            var userData = await FindUserAsync<UserData>(userDataToAdd.UserId, cancellationToken);
            if (!userData.Success)
            {
                return userData;
            }

            userDataToAdd.UserFiles = new List<UserFile>();
            await _context.UserData.AddAsync(userDataToAdd, cancellationToken);
            _context.SaveChanges();

            result.Data = userDataToAdd;
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData>> DeleteUserDataAsync(int userId, int userDataId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId}", userId, userDataId);

        var result = new ResultWrapper<UserData>
        {
            Success = true
        };

        UserData? userDataToDelete = null;

        try
        {
            userDataToDelete = await _context.UserData.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == userDataId,
                cancellationToken);

            if (userDataToDelete is null)
            {
                Helpers.LogNotFoundWarning(result, $"UserDataId {userDataId} not found", _logger);
                return result;
            }

            _context.UserData.Remove(userDataToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            result.Data = userDataToDelete;
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData>> GetUserDataAsync(int userId, int userDataId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId}", userId, userDataId);

        var result = new ResultWrapper<UserData>
        {
            Success = true
        };

        UserData? userData = null;

        try
        {
            userData = await _context.UserData
                .Include(data => data.UserFiles)
                .FirstOrDefaultAsync(y => y.UserId == userId && y.Id == userDataId, cancellationToken);

            if (userData is null)
            {
                Helpers.LogNotFoundWarning(result, $"UserDataId {userDataId} not found", _logger);
                return result;
            }

            await _context.SaveChangesAsync(cancellationToken);

            result.Data = userData;
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(int userId,
            CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId}", userId);

        var result = new ResultWrapper<UserData[]>
        {
            Success = true
        };

        UserData[]? dataList = null;

        try
        {
            var user = await FindUserAsync<UserData[]>(userId, cancellationToken);
            if (!user.Success)
            {
                return user;
            }

            dataList = await _context.UserData
                .Include(data => data.UserFiles)
                .Where(y => y.UserId == userId)
                .ToArrayAsync(cancellationToken);

            result.Data = dataList;
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate,
            CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userId},{userDataId}", userDataToUpdate.UserId, userDataToUpdate.Id);

        var result = new ResultWrapper<UserData>
        {
            Success = true
        };

        UserData? updatedUserData = null;

        try
        {
            var user = await FindUserAsync<UserData>(userDataToUpdate.UserId, cancellationToken);
            if (!user.Success)
            {
                return user;
            }

            updatedUserData = await _context.UserData.FirstOrDefaultAsync(x => x.UserId == userDataToUpdate.UserId && x.Id == userDataToUpdate.Id,
                cancellationToken);

            if (updatedUserData is null)
            {
                Helpers.LogNotFoundWarning(result, $"UserDataId {userDataToUpdate.Id} not found", _logger);
                return result;
            }

            updatedUserData.Title = userDataToUpdate.Title;
            updatedUserData.Data = userDataToUpdate.Data;

            await _context.SaveChangesAsync(cancellationToken);

            result.Data = updatedUserData;
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<User>> GetUserAsync(int userDataId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Started:{userDataId}", userDataId);

        var result = new ResultWrapper<User>();

        try
        {
            IEnumerable<User> res = from data in _context.UserData
                                    join user in _context.Users
                                    on new { x1 = data.UserId, x2 = data.Id } equals new { x1 = user.Id, x2 = userDataId }
                                    select user;

            if (res != null)
            {
                User? foundUser = await res.AsQueryable().FirstOrDefaultAsync(cancellationToken);
                if (foundUser != null)
                {
                    result.Success = true;
                    result.Data = foundUser;
                }
                else
                {
                    Helpers.LogNotFoundWarning(result, $"User owning UserData:{userDataId} not found", _logger);
                }
            }
        }
        catch (Exception ex)
        {
            Helpers.LogException(result, ex, _logger);
        }

        _logger.LogInformation("Finished");

        return result;
    }

    /// <summary>
    /// Searches for usser by Id.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
    /// <returns>ResultWrapper.Success = true if user exists</returns>
    private async Task<ResultWrapper<T>> FindUserAsync<T>(int userId, CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<T>
        {
            Success = true
        };

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
        {
            Helpers.LogNotFoundWarning(result, $"UserId {userId} not found", _logger);
        }

        return result;
    }
}
