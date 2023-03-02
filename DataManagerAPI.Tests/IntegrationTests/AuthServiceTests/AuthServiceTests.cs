using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthServiceTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        DatabaseFixture.PrepareDatabase(factory);
    }

    #region Revoke
    [Fact]
    public async Task Revoke_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.User.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/revoke");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        registredUser.LoginData = null;
    }
    #endregion

    #region Role
    [Fact]
    public async Task UpdateUserRole_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());
        using RegisteredUserTestData userToChange = await UsersForTestsHelper.FindOrCreateRegisteredUser(_client, RoleIds.User.ToString());

        string newRole = "ReadOnlyUser";

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/auth/changerole?userId={userToChange.Id}");
        request.Content = new StringContent(JsonConvert.SerializeObject(newRole), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        userToChange.RegisteredUser.Role = newRole;

        var response = await responseMessage.Content.ReadAsStringAsync();
        Assert.NotNull(response);
        Assert.Equal(newRole, response, true);
    }

    [Fact]
    public async Task UpdateUserRole_Same_Role_Returns_Conflict()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());
        using RegisteredUserTestData userToChange = await UsersForTestsHelper.FindOrCreateRegisteredUser(_client, RoleIds.User.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/auth/changerole?userId={userToChange.Id}");
        request.Content = new StringContent(JsonConvert.SerializeObject(userToChange.RegisteredUser.Role), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task UpdateUserRole_Incorrect_User_Returns_NotFound()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, "api/auth/changerole?userId=100000");
        request.Content = new StringContent(JsonConvert.SerializeObject("ReadOnlyUser"), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage.StatusCode);
    }


    [Fact]
    public async Task UpdateUserRole_For_DefaultAdmin_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());
        string newRole = "ReadOnlyUser";

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, "api/auth/changerole?userId=1");
        request.Content = new StringContent(JsonConvert.SerializeObject(newRole), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion

    #region Logout
    [Fact]
    public async Task Logout_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        // Act
        HttpResponseMessage responseMessage;
        using (var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        responseMessage.Dispose();

        // check that user is unauthorized now
        using (var request = new HttpRequestMessage(HttpMethod.Get, "api/users/all"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
        responseMessage.Dispose();
    }
    #endregion
}
