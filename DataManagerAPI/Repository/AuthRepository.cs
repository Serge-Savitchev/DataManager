using DataManagerAPI.Helpers;
using DataManagerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManagerAPI.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UsersDBContext _context;

        public AuthRepository(UsersDBContext context)
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

        public async Task<ResultWrapper<int>> RefreshToken(int userId, string? token)
        {
            var result = new ResultWrapper<int>();
            try
            {
                var userCredentials = await _context.UserCredentials
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (userCredentials is null)
                {
                    result.Success = false;
                    result.StatusCode = StatusCodes.Status404NotFound;
                    result.Message = $"User with Id {userId} not found.";

                    return result;
                }

                userCredentials.RefreshToken = token;

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

        public async Task<ResultWrapper<int>> UpdateUserPassword(int userId, UserCredentials credentials)
        {
            var result = new ResultWrapper<int>();

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


    }
}
