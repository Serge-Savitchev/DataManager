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

    public async Task<ResultWrapper<List<User>>> GetUsersByRole(RoleIds roleId)
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

    public async Task<ResultWrapper<int>> UpdateOwners(int ownerId, int[] users)
    {
        var result = new ResultWrapper<int>();
        try
        {
            IEnumerable<User> res = from u in users
                                    join user in _context.Users
                                    on u equals user.Id
                                    select user;

            res.AsParallel().ForAll(x => x.OwnerId = ownerId);

            await _context.SaveChangesAsync();
            result.Data = res.Count();
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