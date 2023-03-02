using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.UsersServiceTests;

public class UsersServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UsersServiceTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        DatabaseFixture.PrepareDatabase(_factory);
    }

    #region DeleteUser
    [Fact]
    public async Task DeleteUser_ByAdmin_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        // Act
        HttpResponseMessage responseMessage;

        using (var request = new HttpRequestMessage(HttpMethod.Delete, $"api/users?userId={registredUser.Id}"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.Equal(registredUser.RegisteredUser.FirstName, response.FirstName);

        UsersForTestsHelper.DeleteUser(registredUser);

        responseMessage.Dispose();

        // check that user has been deleted
        using (var request = new HttpRequestMessage(HttpMethod.Delete, $"api/users?userId={registredUser.Id}"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage.StatusCode);
        responseMessage.Dispose();
    }

    [Fact]
    public async Task DeleteUser_ByPowerUser_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/users?userId={registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ByAdmin_Unauthorized_Returns_Unauthorized()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        // Act
        using HttpResponseMessage responseMessage = await _client.DeleteAsync($"api/users?userId={registredUser.Id}");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    #endregion

    #region GetUser

    [Fact]
    public async Task GetUser_Unauthorized_Returns_Unauthorized()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        // Act
        using HttpResponseMessage responseMessage = await _client.GetAsync($"api/users?UserId={registredUser.Id}");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetUser_Returns_User()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/users?UserId={registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.Equal(registredUser.RegisteredUser.LastName, response.LastName);
        Assert.Equal(registredUser.RegisteredUser.Role, response.Role);
    }

    #endregion

    #region GetAllUsers

    [Fact]
    public async Task GetAllUsers_ByAdmin_Returns_AllUsers()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/users/all");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto[] response = await responseMessage.Content.ReadAsAsync<UserDto[]>();
        Assert.True(response.Length > 0);
        Assert.NotNull(response.FirstOrDefault(x => x.Id == registredUser.Id));
    }

    [Fact]
    public async Task GetAllUsers_ByPowerUser_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/users/all");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion

    #region GetUsersByRole

    [Fact]
    public async Task GetUsersByRole_ByAdmin_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());
        using RegisteredUserTestData user = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.User.ToString());

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/users/role?role=user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto[] response = await responseMessage.Content.ReadAsAsync<UserDto[]>();
        Assert.True(response.Length >= 1);
        Assert.NotNull(response.FirstOrDefault(x => x.Id == user.Id));
    }

    [Fact]
    public async Task GetUsersByRole_ByAdmin_InavlidRole_Returns_BadRequest()
    {
        // Arrange
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());
        using RegisteredUserTestData powerUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/users/role?role=badrole");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }


    [Fact]
    public async Task GetUsersByRole_ByPowerUser_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData powerUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/users/role?role=admin");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", powerUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion

    #region UpdateOwners

    [Fact]
    public async Task UpdateOwners_ByPowerUser_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData powerUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());
        using RegisteredUserTestData user1 = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.User.ToString());
        using RegisteredUserTestData user2 = await UsersForTestsHelper.FindOrCreateRegisteredUser(_client, RoleIds.ReadOnlyUser.ToString());

        UpdateOwnerRequestDto requestData = new UpdateOwnerRequestDto
        {
            OwnerId = powerUser.Id,
            UserIds = new int[] { user1.Id, user2.Id }
        };

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/users/updateowners")
        {
            Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", powerUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        int response = await responseMessage.Content.ReadAsAsync<int>();
        Assert.Equal(2, response);

        // check that user1 has changed owner
        using var request0 = new HttpRequestMessage(HttpMethod.Get, $"api/users?UserId={user1.Id}");
        request0.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user1.LoginData!.AccessToken);

        using HttpResponseMessage responseMessage0 = await _client.SendAsync(request0);
        responseMessage0.EnsureSuccessStatusCode();

        UserDto response0 = await responseMessage0.Content.ReadAsAsync<UserDto>();
        Assert.Equal(powerUser.Id, response0.OwnerId);
    }

    [Fact]
    public async Task UpdateOwners_ByUser_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData user = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.User.ToString());
        using RegisteredUserTestData user1 = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.User.ToString());

        UpdateOwnerRequestDto requestData = new UpdateOwnerRequestDto
        {
            OwnerId = user.Id,
            UserIds = new int[] { user1.Id }
        };

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/users/updateowners");
        request.Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion
}
