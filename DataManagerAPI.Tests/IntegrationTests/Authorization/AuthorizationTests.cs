using DataManagerAPI.Dto;
using DataManagerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Xunit;
using static DataManagerAPI.Tests.IntegrationTests.TestWebApplicationFactory;

namespace DataManagerAPI.Tests.IntegrationTests.Authorization;

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

    #region Register
    [Fact]
    public async Task Post_RedisterUser_Returns_NewUser()
    {
        // Arrange
        using RegisterUserTestData requestData = UsersForTestsHelper.GenerateUniqueUserData(RoleIds.Admin.ToString());

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", requestData.UserData);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.NotNull(response);
        Assert.Equal(requestData.UserData!.FirstName, response.FirstName);

        requestData.Id = response.Id;
    }

    [Fact]
    public async Task Post_RedisterUser_UserExists_Returns_Conflict()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.Admin.ToString());

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", registredUser.UserData);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, (int)responseMessage.StatusCode);
    }
    #endregion

    #region Login
    [Fact]
    public async Task Post_LoginUser_ReturnsPairOfTokens()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.User.ToString());

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
        // Arrange
        RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.User.ToString());

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
    #endregion

    #region Refresh
    [Fact]
    public async Task Post_RefreshToken_ReturnsNewPairOfTokens()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());

        // Expiration time of JWT token is stored in seconds.
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
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());

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
    #endregion

    #region Revoke
    [Fact]
    public async Task Post_Revoke_ReturnsOk()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());

        //Act
        HttpResponseMessage responseMessage;

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/revoke");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        registredUser.LoginData = null;
    }
    #endregion

    #region Password
    [Fact]
    public async Task Put_ChangePassword_User_ReturnsOk()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());

        string newPassword = "newPassword";

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Put, "api/auth/changepassword");
        request.Content = new StringContent(JsonConvert.SerializeObject(newPassword), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        HttpResponseMessage responseMessage = await _client.SendAsync(request);

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

    [Fact]
    public async Task Put_ChangePassword_Admin_ReturnsOk()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());
        using RegisterUserTestData userToChange = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.User.ToString());
        string newPassword = "newPassword";

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/auth/changepassword/{userToChange.Id}");
        request.Content = new StringContent(JsonConvert.SerializeObject(newPassword), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        userToChange.UserData.Password = newPassword;

        // check login with new password
        LoginUserDto requestData = new LoginUserDto
        {
            Login = userToChange.UserData.Login,
            Password = newPassword
        };

        responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);
        responseMessage.EnsureSuccessStatusCode();

        LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
        Assert.NotNull(response);
        Assert.NotEmpty(response.AccessToken);
        Assert.NotEmpty(response.RefreshToken);

        userToChange.LoginData = response;
    }

    [Fact]
    public async Task Put_ChangePassword_NotAdmin_ReturnsForbidden()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());
        using RegisterUserTestData userToChange = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.User.ToString());

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/auth/changepassword/{userToChange.Id}");
        request.Content = new StringContent(JsonConvert.SerializeObject("newPassword"), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }
    #endregion

    #region Role
    [Fact]
    public async Task Put_ChangeRole_ReturnsNewRole()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());
        using RegisterUserTestData userToChange = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.User.ToString());

        string newRole = "ReadOnlyUser";

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/auth/changerole/{userToChange.Id}");
        request.Content = new StringContent(JsonConvert.SerializeObject(newRole), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        userToChange.UserData.Role = newRole;

        var response = await responseMessage.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(newRole, response, true);
    }
    #endregion

    #region UserDetails
    [Fact]
    public async Task Get_UserDetails_Admin_ReturnsOk()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/auth/{registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        var response = await responseMessage.Content.ReadAsAsync<UserDetailsDto>();
        Assert.NotNull(response);
        Assert.Equal(registredUser.Id, response.Id);
        Assert.Equal(registredUser.UserData.LastName, response.LastName);
        Assert.Equal(registredUser.UserData.Login, response.Login);
    }

    [Fact]
    public async Task Get_UserDetails_NotAdmin_ReturnsForbidden()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/auth/{registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion
}
