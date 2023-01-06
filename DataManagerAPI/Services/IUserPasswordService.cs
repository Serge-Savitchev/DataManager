using DataManagerAPI.Repository.Models;

namespace DataManagerAPI.Services
{
    public interface IUserPasswordService
    {
        UserCredentials CreatePasswordHash(string password);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
