using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManagerAPI.SQLServerDB.Implementation;

/// <summary>
/// Implementation of <see cref="IUserDataRepository"/>.
/// </summary>
public class UserDataRepository : IUserDataRepository
{
    private readonly UsersDBContext _context;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="context"><see cref="UsersDBContext"/></param>
    public UserDataRepository(UsersDBContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData>> AddUserDataAsync(UserData userDataToAdd,
        CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserData>
        {
            StatusCode = StatusCodes.Status201Created,
            Success = true
        };

        try
        {
            var userData = await FindUserAsync<UserData>(userDataToAdd.UserId, cancellationToken);
            if (!userData.Success)
            {
                return userData;
            }

            await _context.UserData.AddAsync(userDataToAdd, cancellationToken);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        result.Data = userDataToAdd;
        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData>> DeleteUserDataAsync(int userDataId,
        CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserData>
        {
            Success = true
        };

        UserData? userDataToDelete = null;

        try
        {
            userDataToDelete = await _context.UserData.FirstOrDefaultAsync(x => x.Id == userDataId, cancellationToken);
            if (userDataToDelete is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserDataId {userDataId} not found";

                return result;
            }

            _context.UserData.Remove(userDataToDelete);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        result.Data = userDataToDelete;
        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData>> GetUserDataAsync(int userDataId,
        CancellationToken cancellationToken = default)
    {
        var result = new ResultWrapper<UserData>
        {
            Success = true
        };

        UserData? userData = null;

        try
        {
            userData = await _context.UserData
                .Include(data => data.UserFiles)
                .FirstOrDefaultAsync(y => y.Id == userDataId, cancellationToken);

            if (userData is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserDataId {userDataId} not found";

                return result;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        result.Data = userData;
        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData[]>> GetUserDataByUserIdAsync(int userId,
            CancellationToken cancellationToken = default)
    {
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
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        result.Data = dataList;
        return result;
    }

    /// <inheritdoc />
    public async Task<ResultWrapper<UserData>> UpdateUserDataAsync(UserData userDataToUpdate,
            CancellationToken cancellationToken = default)
    {
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

            updatedUserData = await _context.UserData.FirstOrDefaultAsync(x => x.Id == userDataToUpdate.Id, cancellationToken);
            if (updatedUserData is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserDataId {userDataToUpdate.Id} not found";

                return result;
            }

            updatedUserData.Title = userDataToUpdate.Title;
            updatedUserData.Data = userDataToUpdate.Data;

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        result.Data = updatedUserData;
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

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user is null)
            {
                throw new Exception();
            }
        }
        catch (Exception)
        {
            result.Success = false;
            result.StatusCode = StatusCodes.Status404NotFound;
            result.Message = $"UserId {userId} not found";
        }

        return result;
    }
}
