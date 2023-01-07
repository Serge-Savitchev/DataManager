using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using static DataManagerAPI.Tests.IntegrationTests.TestWebApplicationFactory;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthServiceTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        DatabaseFixture.PrepareDatabase(_factory);
    }

    #region Revoke
    [Fact]
    public async Task Post_Revoke_ReturnsOk()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/revoke");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        registredUser.LoginData = null;
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

    #region Logout
    [Fact]
    public async Task Post_Logout_ReturnsOk()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());

        //Act
        HttpResponseMessage responseMessage;
        using (var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        // check that user is unauthorized now
        using (var request = new HttpRequestMessage(HttpMethod.Get, "api/user/all"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }
    #endregion
}
