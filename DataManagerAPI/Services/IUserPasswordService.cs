using DataManagerAPI.Repository.Abstractions.Models;

namespace DataManagerAPI.Services;

public interface IUserPasswordService
{
    UserCredentials CreatePasswordHash(string password);
    bool VerifyPasswordHash(string password, string passwordHash);
}
