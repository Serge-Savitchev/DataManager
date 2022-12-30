using DataManagerAPI.Dto;
using DataManagerAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static DataManagerAPI.Integration.Tests.TestWebApplicationFactory;

namespace DataManagerAPI.Integration.Tests;

public class DatabaseFixture
{
    public const string ConnectionString = "Server=.;Database=TestDB;Trusted_Connection=true;Trust Server Certificate=true";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;
    private static int _uniqueUserNumber;

    public static void PrepareDatabase(CustomWebApplicationFactory<Program> factory)
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var scope = factory.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<UsersDBContext>();
                    db.Database.EnsureDeleted();
                    db.Database.Migrate();
                }

                _databaseInitialized = true;
            }
        }
    }

    public static RegisterUserDto GenerateUniqueUserData(string role)
    {
        lock (_lock)
        {
            _uniqueUserNumber++;
            string newNumber = _uniqueUserNumber.ToString("D4");

            RegisterUserDto ret = new RegisterUserDto
            {
                FirstName = $"FirstName{newNumber}",
                LastName = $"LastName{newNumber}",
                Email = $"a{newNumber}@a.com",
                Login = $"Login{newNumber}",
                Password = $"Password{newNumber}",
                Role = role
            };

            return ret;
        }
    }
}