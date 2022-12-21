using DataManagerAPI.Helpers;
using DataManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManagerAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly UsersDBContext _context;

    public UserRepository(UsersDBContext context)
    {
        _context = context;
    }

    public async Task<ResultWrapper<User>> AddUser(User userToAdd)
    {
        var result = new ResultWrapper<User>
        {
            StatusCode = StatusCodes.Status201Created
        };

        try
        {
            await _context.Users.AddAsync(userToAdd);
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

    public async Task<ResultWrapper<UserCredentials>> GetUserCredentials(int userId)
    {
        var result = new ResultWrapper<UserCredentials>();
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

            result.Data = user?.UserCredentials;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    public async Task<ResultWrapper<User>> UpdateUserCredentials(int userId, UserCredentials credentials)
    {
        var result = new ResultWrapper<User>();

        try
        {
            var updatedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (updatedUser is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }

            updatedUser.UserCredentials = credentials;

            await _context.SaveChangesAsync();

            result.Data = updatedUser;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }


    public async Task<ResultWrapper<User>> UpdateUser(User userToUpdate)
    {
        var result = new ResultWrapper<User>();

        try
        {
            var updatedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userToUpdate.Id);
            if (updatedUser is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserId {userToUpdate.Id} not found";

                return result;
            }

            updatedUser.FirstName = userToUpdate.FirstName;
            updatedUser.LastName = userToUpdate.LastName;
            updatedUser.Email = userToUpdate.Email;
            updatedUser.Role = userToUpdate.Role;

            await _context.SaveChangesAsync();
            result.Data = updatedUser;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    public async Task<ResultWrapper<User>> DeleteUser(int userId)
    {
        var result = new ResultWrapper<User>();

        try
        {
            var userToDelete = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (userToDelete is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            result.Data = userToDelete;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    public async Task<ResultWrapper<User>> GetUser(int userId)
    {
        var result = new ResultWrapper<User>();

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
            result.Data = user;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    public async Task<ResultWrapper<List<User>>> GetUsersByRole(RoleId roleId)
    {
        var result = new ResultWrapper<List<User>>();

        try
        {
            var users = await _context.Users.Where(x => x.Role == roleId).ToListAsync();
            result.Data = users;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    public async Task<ResultWrapper<List<User>>> GetAllUsers()
    {
        var result = new ResultWrapper<List<User>>();

        try
        {
            var users = await _context.Users.ToListAsync();
            result.Data = users;
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
