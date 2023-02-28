using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DataManagerAPI.Tests.IntegrationTests;

internal static class UsersForTestsHelper
{
    private static readonly object _lockRegisterList = new();
    private static int _uniqueUserNumber;

    public static RegisteredUserTestData GenerateUniqueUserData(string role)
    {
        lock (_lockRegisterList)
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

            var ret = new RegisteredUserTestData { Locked = true, RegisterUser = user };
            _registerList.Add(ret);

            return ret;
        }
    }

    private static readonly List<RegisteredUserTestData> _registerList = new()
    {
        // add default admin
        new RegisteredUserTestData
        {
            Locked = true,
            Id = 1,
            RegisterUser = new RegisterUserDto
            {
                FirstName = "DefaultAdmin",
                LastName = "DefaultAdmin",
                Login = "Admin",
                Password = "Admin",
                Role = "Admin"
            }
        }
    };

    private static RegisteredUserTestData? FindRegisteredUser(Func<RegisteredUserTestData, bool> predicate)
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

    public static async Task<RegisteredUserTestData> FindOrCreateRegisteredUser(HttpClient client, string role)
    {
        RegisteredUserTestData? registredUser = null;

        try
        {
            registredUser = FindRegisteredUser(x => x.LoginData == null && !x.Locked && role.Equals(x.RegisterUser.Role, StringComparison.InvariantCultureIgnoreCase));

            if (registredUser == null)
            {
                registredUser = GenerateUniqueUserData(role);
                registredUser = await RegisterNewUser(client, registredUser);
            }
        }
        catch
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }

            throw;
        }

        return registredUser!;
    }

    private static async Task<RegisteredUserTestData?> RegisterNewUser(HttpClient client, RegisteredUserTestData? newUser)
    {
        RegisteredUserTestData defaultAdmin = _registerList[0];

        if (defaultAdmin.LoginData == null)     // login default user
        {
            var requestData = new LoginUserDto
            {
                Login = defaultAdmin.RegisterUser.Login,
                Password = defaultAdmin.RegisterUser.Password
            };

            using var responseMessage1 = await client.PostAsJsonAsync("api/auth/login", requestData);
            LoginUserResponseDto response = await responseMessage1.Content.ReadAsAsync<LoginUserResponseDto>();
            defaultAdmin.LoginData = response;
        }

        if (newUser == null)
        {
            return null;
        }

        var role = Enum.Parse<RoleIds>(newUser.RegisterUser.Role, true);

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/register")
        {
            Content = new StringContent(JsonConvert.SerializeObject(newUser.RegisterUser), Encoding.UTF8, "application/json")
        };

        // register user with "Admin" and "PowerUser" roles
        // requires authentication of default admin
        if (role == RoleIds.Admin || role == RoleIds.PowerUser)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", defaultAdmin.LoginData!.AccessToken);
        }
        using HttpResponseMessage responseMessage = await client.SendAsync(request);

        UserDto user = await responseMessage.Content.ReadAsAsync<UserDto>();
        newUser.Id = user.Id;

        return newUser;
    }

    public static async Task<RegisteredUserTestData> CreateNewLoggedInUser(HttpClient client, string role)
    {
        RegisteredUserTestData? registeredUser = null;
        try
        {
            registeredUser = GenerateUniqueUserData(role);
            registeredUser = await RegisterNewUser(client, registeredUser);

            var requestData = new LoginUserDto
            {
                Login = registeredUser!.RegisterUser.Login,
                Password = registeredUser.RegisterUser.Password
            };

            using var responseMessage1 = await client.PostAsJsonAsync("api/auth/login", requestData);
            LoginUserResponseDto response = await responseMessage1.Content.ReadAsAsync<LoginUserResponseDto>();
            registeredUser.LoginData = response;
        }
        catch
        {
            if (registeredUser != null)
            {
                registeredUser.Locked = false;
            }

            throw;
        }

        return registeredUser!;
    }

    public static async Task<RegisteredUserTestData> FindOrCreateLoggedInUser(HttpClient client, string role)
    {
        RegisteredUserTestData? registredUser = null;
        try
        {
            registredUser = FindRegisteredUser(x => x.LoginData != null && !x.Locked && role.Equals(x.RegisterUser.Role, StringComparison.InvariantCultureIgnoreCase));

            if (registredUser == null)
            {
                registredUser = await CreateNewLoggedInUser(client, role);
            }
        }
        catch
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }

            throw;
        }

        return registredUser!;
    }

    public static void DeleteUser(RegisteredUserTestData user)
    {
        if (user.Id != 1)   // don't remove default user
        {
            _registerList.Remove(user);
        }
    }
}
