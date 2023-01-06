using DataManagerAPI.Repository.Models;
using System.Security.Cryptography;
using System.Text;

namespace DataManagerAPI.Services
{
    public class UserPasswordService : IUserPasswordService
    {
        public UserCredentials CreatePasswordHash(string password)
        {
            var result = new UserCredentials();

            if (!string.IsNullOrWhiteSpace(password))
            {
                using var hmac = new HMACSHA512();
                result.PasswordSalt = hmac.Key;
                result.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return result;
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
