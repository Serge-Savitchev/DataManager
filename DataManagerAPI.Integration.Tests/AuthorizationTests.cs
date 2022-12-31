using DataManagerAPI.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Xunit;
using static DataManagerAPI.Integration.Tests.TestWebApplicationFactory;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DataManagerAPI.Integration.Tests;

public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthorizationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        DatabaseFixture.PrepareDatabase(_factory);
    }

    [Fact]
    public async Task Post_RedisterUser_Returns_NewUser()
    {
        RegisterUserTestData? requestData = null;

        try
        {
            // Arrange
            requestData = UsersForTestsHelper.GenerateUniqueUserData("Admin");

            //Act
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", requestData.UserData);

            // Assert
            responseMessage.EnsureSuccessStatusCode();

            UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
            Assert.NotNull(response);
            Assert.Equal(requestData.UserData!.FirstName, response.FirstName);

            requestData.Id = response.Id;
        }
        finally
        {
            if (requestData != null)
            {
                requestData.Locked = false;
            }
        }
    }

    [Fact]
    public async Task Post_RedisterUser_UserExists_Returns_Conflict()
    {
        RegisterUserTestData? registredUser = null;

        try
        {
            // Arrange
            registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, "admin");

            //Act
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", registredUser.UserData);

            // Assert

            Assert.Equal(StatusCodes.Status409Conflict, (int)responseMessage.StatusCode);
        }
        finally
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }
        }
    }

    [Fact]
    public async Task Post_LoginUser_ReturnsPairOfTokens()
    {
        RegisterUserTestData? registredUser = null;

        try
        {
            // Arrange
            registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, "user");

            LoginUserDto requestData = new LoginUserDto
            {
                Login = registredUser.UserData.Login,
                Password = registredUser.UserData.Password
            };

            //Act
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

            // Assert
            responseMessage.EnsureSuccessStatusCode();

            LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
            Assert.NotNull(response);
            Assert.NotEmpty(response.AccessToken);
            Assert.NotEmpty(response.RefreshToken);

            registredUser.LoginData = response;
        }
        finally
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }
        }
    }

    [Fact]
    public async Task Post_LoginUser_IncorrectLogin_ReturnsNotFound()
    {
        // Arrange
        LoginUserDto requestData = new LoginUserDto
        {
            Login = "Incorrect",
            Password = "fake"
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

        // Assert

        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Post_LoginUser_IncorrectPassword_ReturnsUnauthorized()
    {
        RegisterUserTestData? registredUser = null;

        try
        {
            // Arrange
            registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, "user");

            LoginUserDto requestData = new LoginUserDto
            {
                Login = registredUser.UserData.Login,
                Password = $"{registredUser.UserData.Password}Incorrect"
            };

            //Act
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

            // Assert

            Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
        }
        finally
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }
        }
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "fake")]
    [InlineData("fake", null)]
    [InlineData("", "fake")]
    [InlineData("fake", "")]
    [InlineData("", "")]
    public async Task Post_LoginUser_IncorrectRequest_ReturnsBadRequest(string login, string password)
    {
        // Arrange
        LoginUserDto? request = null;

        if (login != null || password != null)
        {
            request = new LoginUserDto { Login = login!, Password = password };
        }

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", request);

        // Assert

        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Post_RefreshToken_ReturnsNewPairOfTokens()
    {
        RegisterUserTestData? registredUser = null;

        try
        {
            // Arrange
            registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, "user");

            // Expiration time in JWT token is stored in seconds.
            // If time passed between Login and Refresh is < 1 second,
            // old and new tokens will be equal. Have a break 2 seconds to avoid such case.
            Thread.Sleep(2000); // 2 seconds

            TokenApiModelDto requestData = new TokenApiModelDto
            {
                AccessToken = registredUser.LoginData!.AccessToken,
                RefreshToken = registredUser.LoginData.RefreshToken
            };

            //Act
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

            // Assert
            responseMessage.EnsureSuccessStatusCode();

            TokenApiModelDto response = await responseMessage.Content.ReadAsAsync<TokenApiModelDto>();
            Assert.NotNull(response);
            Assert.NotEmpty(response.AccessToken);
            Assert.NotEmpty(response.RefreshToken);
            Assert.NotEqual(requestData.AccessToken, response.AccessToken);
            Assert.NotEqual(requestData.RefreshToken, response.RefreshToken);

            registredUser.LoginData = new LoginUserResponseDto
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken
            };
        }
        finally
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }
        }
    }

    [Fact]
    public async Task Post_RefreshToken_IncorrectAccessToken_ReturnsUnauthorized()
    {
        // Arrange
        TokenApiModelDto requestData = new TokenApiModelDto
        {
            AccessToken = "Incorrect",
            RefreshToken = "fake"
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

        // Assert

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Post_RefreshToken_IncorrectRefreshToken_ReturnsUnauthorized()
    {
        RegisterUserTestData? registredUser = null;

        try
        {
            // Arrange
            registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, "user");

            TokenApiModelDto requestData = new TokenApiModelDto
            {
                AccessToken = registredUser.LoginData!.AccessToken,
                RefreshToken = "fake"
            };

            //Act
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

            // Assert

            Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
        }
        finally
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }
        }
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "fake")]
    [InlineData("fake", null)]
    [InlineData("", "fake")]
    [InlineData("fake", "")]
    [InlineData("", "")]
    public async Task Post_RefreshToken_IncorrectRequest_ReturnsBadRequest(string accessToken, string refreshToken)
    {
        // Arrange
        TokenApiModelDto? request = null;
        if (accessToken != null || refreshToken != null)
        {
            request = new TokenApiModelDto { AccessToken = accessToken!, RefreshToken = refreshToken };
        }

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", request);

        // Assert

        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Post_Revoke_ReturnsOk()
    {
        RegisterUserTestData? registredUser = null;

        try
        {
            // Arrange
            registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, "user");

            //Act
            HttpResponseMessage responseMessage;

            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/revoke"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
                responseMessage = await _client.SendAsync(request);
            }

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            registredUser.LoginData = null;
        }
        finally
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }
        }
    }

    [Fact]
    public async Task Post_ChangePassword_ReturnsOk()
    {
        RegisterUserTestData? registredUser = null;

        try
        {
            // Arrange
            registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, "user");
            string oldPassword = registredUser.UserData.Password;
            string newPassword = "newPassword";

            //Act
            HttpResponseMessage responseMessage;

            using (var request = new HttpRequestMessage(HttpMethod.Put, $"api/auth/credentials/{registredUser.Id}"))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(newPassword), Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
                responseMessage = await _client.SendAsync(request);
            }

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            registredUser.UserData.Password = newPassword;

            // check login with new password
            LoginUserDto requestData = new LoginUserDto
            {
                Login = registredUser.UserData.Login,
                Password = newPassword
            };

            responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);
            responseMessage.EnsureSuccessStatusCode();

            LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
            Assert.NotNull(response);
            Assert.NotEmpty(response.AccessToken);
            Assert.NotEmpty(response.RefreshToken);

            registredUser.LoginData = response;
        }
        finally
        {
            if (registredUser != null)
            {
                registredUser.Locked = false;
            }
        }
    }

}
