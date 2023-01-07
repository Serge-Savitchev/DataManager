using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using Xunit;
using static DataManagerAPI.Tests.IntegrationTests.TestWebApplicationFactory;

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

        using (var request = new HttpRequestMessage(HttpMethod.Delete, $"api/user/{registredUser.Id}"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.Equal(registredUser.UserData.FirstName, response.FirstName);

        UsersForTestsHelper.DeleteUser(registredUser);

        // check that user has been deleted
        using (var request = new HttpRequestMessage(HttpMethod.Delete, $"api/user/{registredUser.Id}"))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
            responseMessage = await _client.SendAsync(request);
        }

        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Delete_ByPowerUser_ReturnsForbidden()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());

        //Act
        var request = new HttpRequestMessage(HttpMethod.Delete, $"api/user/{registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Delete_ByAdmin_Unauthorized_ReturnsUnauthorized()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());

        //Act
        HttpResponseMessage responseMessage = await _client.DeleteAsync($"api/user/{registredUser.Id}");

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
        HttpResponseMessage responseMessage = await _client.GetAsync($"api/user/{registredUser.Id}");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Get_ReturnsUser()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());

        //Act
        var request = new HttpRequestMessage(HttpMethod.Get, $"api/user/{registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.Equal(registredUser.UserData.LastName, response.LastName);
        Assert.Equal(registredUser.UserData.Role, response.Role);
    }

    #endregion

}
