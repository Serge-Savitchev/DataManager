using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.UserDataServiceTests;

public partial class UserDataServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region DeleteUserData

    [Theory]
    [InlineData("{userId}/{UserDataId}")]
    [InlineData("0/{UserDataId}")]
    public async Task DeleteUserData_Own_Returns_Ok(string queryValue)
    {
        // Arrange
        using RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIds.User.ToString());
        var data = new AddUserDataDto
        {
            Title = "User Test Title",
            Data = "User Test Data"
        };

        // add new user data
        UserDataDto response0 = await AddNewUserData(user, data);

        // Act
        string query = "api/userdata/" +
            queryValue
            .Replace("{userId}", user.Id.ToString())
            .Replace("{UserDataId}", response0.Id.ToString());

        using var request = new HttpRequestMessage(HttpMethod.Delete, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        // check data
        UserDataDto response = await responseMessage.Content.ReadAsAsync<UserDataDto>();

        Assert.NotNull(response);
        Assert.Equal(response0.Id, response.Id);
        Assert.Equal(data.Title, response.Title);
        Assert.Equal(data.Data, response.Data);

        // check that data has been deleted
        string query2 = $"api/userdata/{user.Id}/{response0.Id}";
        using var request2 = new HttpRequestMessage(HttpMethod.Get, query2);

        request2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage2 = await _client.SendAsync(request2);

        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage2.StatusCode);
    }

    [Fact]
    public async Task DeleteUserData_Admin_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());
        using RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIds.User.ToString());
        var data = new AddUserDataDto
        {
            Title = "User Test Title!",
            Data = "User Test Data!"
        };

        // add new user data
        UserDataDto response0 = await AddNewUserData(user, data);

        // Act
        string query = $"api/userdata/{user.Id}/{response0.Id}";
        using var request = new HttpRequestMessage(HttpMethod.Delete, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        // check data
        UserDataDto response = await responseMessage.Content.ReadAsAsync<UserDataDto>();

        Assert.NotNull(response);
        Assert.Equal(response0.Id, response.Id);
        Assert.Equal(data.Title, response.Title);
        Assert.Equal(data.Data, response.Data);

        // check that data has been deleted
        string query2 = $"api/userdata/{user.Id}/{response0.Id}";
        using var request2 = new HttpRequestMessage(HttpMethod.Get, query2);

        request2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage2 = await _client.SendAsync(request2);

        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage2.StatusCode);
    }

    [Fact]
    public async Task DeleteUserData_IncorrectId_Returns_NotFound()
    {
        // Arrange
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        // Act
        string query = "api/userdata/1/1";
        using var request = new HttpRequestMessage(HttpMethod.Delete, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task DeleteUserData_Unauthorized_Returns_Unauthorized()
    {
        // Act
        using HttpResponseMessage responseMessage = await _client.DeleteAsync("api/userdata/1/1");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task DeleteUserData_NotOwn_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIds.PowerUser.ToString());

        string query = "api/userdata/1/1";
        using var request = new HttpRequestMessage(HttpMethod.Delete, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion
}
