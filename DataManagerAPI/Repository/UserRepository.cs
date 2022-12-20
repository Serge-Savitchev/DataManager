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
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        result.Data = userToAdd;
        return result;
    }

    public async Task<UserCredentials?> GetUserCredentions(int userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        return user?.UserCredentials;
    }
}
