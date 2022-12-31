using DataManagerAPI.Dto;
using DataManagerAPI.Models;
using DataManagerAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static DataManagerAPI.Integration.Tests.TestWebApplicationFactory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataManagerAPI.Integration.Tests;

public class DatabaseFixture
{
    public const string ConnectionString = "Server=.;Database=TestDB;Trusted_Connection=true;Trust Server Certificate=true";

    private static readonly object _lockDB = new();
    private static bool _databaseInitialized;
    private static int _uniqueUserNumber;

    public static void PrepareDatabase(CustomWebApplicationFactory<Program> factory)
    {
        lock (_lockDB)
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

    private static readonly object _lockNewUser = new();

    public static RegisterUserTestData GenerateUniqueUserData(string role)
    {
        lock (_lockNewUser)
        {
            _uniqueUserNumber++;
            string newNumber = _uniqueUserNumber.ToString("D4");

            RegisterUserDto user = new RegisterUserDto
            {
                FirstName = $"FirstName{newNumber}",
                LastName = $"LastName{newNumber}",
                Email = $"a{newNumber}@a.com",
                Login = $"Login{newNumber}",
                Password = $"Password{newNumber}",
                Role = role
            };

            var ret = new RegisterUserTestData { Locked = true, UserData = user };
            _registerList.Add(ret);

            return ret;
        }
    }

    private static readonly object _lockRegisterList = new();

    private static List<RegisterUserTestData> _registerList = new List<RegisterUserTestData>();

    //public static RegisterUserTestData AddRegisterUser(int userId, RegisterUserDto user, LoginUserResponseDto? loginData = null)
    //{
    //    lock (_lockRegisterList)
    //    {
    //        var data = new RegisterUserTestData { Id = userId, UserData = user, LoginData = loginData };
    //        _registerList.Add(data);
    //        return data;
    //    }
    //}

    public static RegisterUserTestData? FindRegisterUser(Func<RegisterUserTestData, bool> predicate)
    {
        lock (_lockRegisterList)
        {
            var ret = _registerList.FirstOrDefault(predicate);
            if (ret is not null)
            {
                ret.Locked = true;
            }

            return ret;
        }
    }

}