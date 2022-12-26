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

    public async Task<ResultWrapper<User>> RegisterUser(User userToAdd, UserCredentials userCredentials)
    {
        var result = new ResultWrapper<User>
        {
            StatusCode = StatusCodes.Status201Created
        };

        try
        {
            var existingCredentials = await _context.UserCredentials
                .FirstOrDefaultAsync(x => x.Login == userCredentials.Login);

            if (existingCredentials != null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status409Conflict;
                result.Message = $"User with login {userCredentials.Login} already exists.";
                return result;
            }

            await _context.Users.AddAsync(userToAdd);
            await _context.SaveChangesAsync();

            userCredentials.UserId = userToAdd.Id;
            await _context.UserCredentials.AddAsync(userCredentials);
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

    public async Task<ResultWrapper<int>> Login(string login, UserCredentials credentials)
    {
        var result = new ResultWrapper<int>();

        try
        {
            var userCredentials = await _context.UserCredentials
                .FirstOrDefaultAsync(x => x.Login == login);

            if (userCredentials is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"User with login {login} not found.";

                return result;
            }

            userCredentials.RefreshToken = credentials.RefreshToken;

            await _context.SaveChangesAsync();
            result.Data = userCredentials.UserId;
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
            var userCredentials = await _context.UserCredentials.FirstOrDefaultAsync(x => x.UserId == userId);

            if (userCredentials is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }

            result.Data = userCredentials;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    public async Task<ResultWrapper<string>> UpdateUserPassword(int userId, UserCredentials credentials)
    {
        var result = new ResultWrapper<string>();

        try
        {
            var updatedCredentials = await _context.UserCredentials.FirstOrDefaultAsync(x => x.UserId == userId);
            if (updatedCredentials is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"UserId {userId} not found";

                return result;
            }

            updatedCredentials.PasswordSalt = credentials.PasswordSalt;
            updatedCredentials.PasswordHash = credentials.PasswordHash;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return result;
    }

    /*
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
    */
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

    public async Task<ResultWrapper<UserCredentialsData>> GetUserByLogin(string login)
    {
        var result = new ResultWrapper<UserCredentialsData>
        {
            Data = new UserCredentialsData()
        };

        try
        {
            var userCredentials = await _context.UserCredentials
                .FirstOrDefaultAsync(x => x.Login == login);

            if (userCredentials is null)
            {
                result.Success = false;
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Message = $"User with login {login} not found.";

                return result;
            }

            result.Data.User = await _context.Users.FirstAsync(x => x.Id == userCredentials.UserId);
            result.Data.Credentials = userCredentials;
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