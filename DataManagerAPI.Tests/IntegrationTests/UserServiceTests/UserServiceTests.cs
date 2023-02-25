using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.UserServiceTests;

public class UserServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UserServiceTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        DatabaseFixture.PrepareDatabase(_factory);
    }

    #region Delete
    [Fact]
    public async Task Delete_ByAdmin_ReturnsDeletedUser()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());

        //Act
        HttpResponseMessage responseMessage;

        using (var request = new HttpRequestMessage(HttpMethod.Delete, $"api/users?userId={registredUser.Id}"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.Equal(registredUser.UserData.FirstName, response.FirstName);

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
    public async Task Delete_ByPowerUser_ReturnsForbidden()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/users?userId={registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Delete_ByAdmin_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());

        //Act
        using HttpResponseMessage responseMessage = await _client.DeleteAsync($"api/users?userId={registredUser.Id}");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    #endregion

    #region GetUser

    [Fact]
    public async Task Get_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());

        //Act
        using HttpResponseMessage responseMessage = await _client.GetAsync($"api/users?UserId={registredUser.Id}");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsUser()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());

        //Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/users?UserId={registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.Equal(registredUser.UserData.LastName, response.LastName);
        Assert.Equal(registredUser.UserData.Role, response.Role);
    }

    #endregion

    #region GetAllUsers

    [Fact]
    public async Task GetAllUsers_ByAdmin_Returns_AllUsers()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());

        //Act
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
    public async Task GetAllUsers_ByPowerUser_ReturnsForbidden()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/users/all");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
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
        using RegisterUserTestData powerUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());
        using RegisterUserTestData user1 = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());
        using RegisterUserTestData user2 = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.ReadOnlyUser.ToString());

        UpdateOwnerRequestDto requestData = new UpdateOwnerRequestDto
        {
            OwnerId = powerUser.Id,
            UserIds = new int[] { user1.Id, user2.Id }
        };

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/users/updateowners");
        request.Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
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
    public async Task UpdateOwners_ByUser_ReturnsForbidden()
    {
        // Arrange
        using RegisterUserTestData user = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());
        using RegisterUserTestData user1 = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());

        UpdateOwnerRequestDto requestData = new UpdateOwnerRequestDto
        {
            OwnerId = user.Id,
            UserIds = new int[] { user1.Id }
        };

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/users/updateowners");
        request.Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion
}
