using DataManagerAPI.Helpers;
using DataManagerAPI.Models;

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
        var result = new ResultWrapper<User>();

        //if (!Enum.IsDefined(userToAdd.Role))
        //{
        //    result.Success = false;
        //    result.Message = "Invalid role value";
        //    result.StatusCode = StatusCodes.Status400BadRequest;
        //    return result;
        //}
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
}
