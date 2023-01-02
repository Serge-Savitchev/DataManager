using DataManagerAPI.Dto;

namespace DataManagerAPI.Tests.IntegrationTests;

internal static class UsersForTestsHelper
{
    private static readonly object _lockRegisterList = new();
    private static int _uniqueUserNumber;

    public static RegisterUserTestData GenerateUniqueUserData(string role)
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

            var ret = new RegisterUserTestData { Locked = true, UserData = user };
            _registerList.Add(ret);

            return ret;
        }
    }

    private static List<RegisterUserTestData> _registerList = new List<RegisterUserTestData>();

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

    public static async Task<RegisterUserTestData> FindOrCreateRegistredUser(HttpClient client, string role)
    {
        RegisterUserTestData? registredUser = null;

        try
        {
            registredUser = FindRegisterUser(x => x.LoginData == null && !x.Locked && role.Equals(x.UserData.Role, StringComparison.InvariantCultureIgnoreCase));

            if (registredUser == null)
            {
                registredUser = GenerateUniqueUserData(role);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync("api/auth/register", registredUser.UserData);
                UserDto user = await responseMessage.Content.ReadAsAsync<UserDto>();
                registredUser.Id = user.Id;
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

    public static async Task<RegisterUserTestData> FindOrCreateLoggedUser(HttpClient client, string role)
    {
        RegisterUserTestData? registredUser = null;
        try
        {
            registredUser = FindRegisterUser(x => x.LoginData != null && !x.Locked && role.Equals(x.UserData.Role, StringComparison.InvariantCultureIgnoreCase));

            if (registredUser == null)
            {
                registredUser = GenerateUniqueUserData(role);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync("api/auth/register", registredUser.UserData);
                UserDto user = await responseMessage.Content.ReadAsAsync<UserDto>();
                registredUser.Id = user.Id;

                LoginUserDto requestData = new LoginUserDto
                {
                    Login = registredUser.UserData.Login,
                    Password = registredUser.UserData.Password
                };

                responseMessage = await client.PostAsJsonAsync("api/auth/login", requestData);
                LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
                registredUser.LoginData = response;
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

}
