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
}
