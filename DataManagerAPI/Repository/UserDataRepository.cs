using DataManagerAPI.Helpers;
using DataManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManagerAPI.Repository;

public class UserDataRepository : IUserDataRepository
{
    private readonly UsersDBContext _context;

    public UserDataRepository(UsersDBContext context)
    {
        _context = context;
    }

    public async Task<ResultWrapper<UserData>> AddUserData(UserData userDataToAdd)
    {
        var result = new ResultWrapper<UserData>
        {
            StatusCode = StatusCodes.Status201Created
        };

        try
        {
            var user = await FindUser<UserData>(userDataToAdd.UserId);
            if (!user.Success)
            {
                return user;
            }

            await _context.UserData.AddAsync(userDataToAdd);
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

    public async Task<ResultWrapper<UserData>> DeleteUserData(int userDataId)
    {
        var result = new ResultWrapper<UserData>();
        UserData? userDataToDelete = null;

        try
        {
            userDataToDelete = await _context.UserData.FirstOrDefaultAsync(x => x.Id == userDataId);
            if (userDataToDelete is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserDataId {userDataId} not found";

                return result;
            }

            _context.UserData.Remove(userDataToDelete);
            await _context.SaveChangesAsync();
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

    public async Task<ResultWrapper<UserData>> GetUserData(int userDataId)
    {
        var result = new ResultWrapper<UserData>();
        UserData? userData = null;

        try
        {
            userData = await _context.UserData.FirstOrDefaultAsync(x => x.Id == userDataId);
            if (userData is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserDataId {userDataId} not found";

                return result;
            }

            await _context.SaveChangesAsync();
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

    public async Task<ResultWrapper<List<UserData>>> GetUserDataByUserId(int userId)
    {
        var result = new ResultWrapper<List<UserData>>();
        List<UserData>? dataList = null;

        try
        {
            var user = await FindUser<List<UserData>>(userId);
            if (!user.Success)
            {
                return user;
            }

            dataList = await _context.UserData.Where(x => x.UserId == userId).ToListAsync();
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

    public async Task<ResultWrapper<UserData>> UpdateUserData(UserData userDataToUpdate)
    {
        var result = new ResultWrapper<UserData>();
        UserData? updatedUserData = null;

        try
        {
            var user = await FindUser<UserData>(userDataToUpdate.UserId);
            if (!user.Success)
            {
                return user;
            }

            updatedUserData = await _context.UserData.FirstOrDefaultAsync(x => x.Id == userDataToUpdate.Id);
            if (updatedUserData is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserDataId {userDataToUpdate.Id} not found";

                return result;
            }

            updatedUserData.Title = userDataToUpdate.Title;
            updatedUserData.Data = userDataToUpdate.Data;

            await _context.SaveChangesAsync();
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

    private async Task<ResultWrapper<T>> FindUser<T>(int userId)
    {
        var result = new ResultWrapper<T>();
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
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
