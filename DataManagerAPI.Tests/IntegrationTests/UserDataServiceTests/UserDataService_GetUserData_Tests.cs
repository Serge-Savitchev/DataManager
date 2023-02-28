using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.UserDataServiceTests;

public partial class UserDataServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region GetUserData

    [Theory]
    [InlineData("UserId={userId}&UserDataId={UserDataId}")]
    [InlineData("UserDataId={UserDataId}")]
    public async Task GetUserData_Own_Returns_Ok(string queryValue)
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIds.User.ToString());
        var data = new AddUserDataDto
        {
            Title = "User Test Title",
            Data = "User Test Data"
        };

        // add new user data
        UserDataDto response0 = await AddNewUserData(registredUser, data);

        // Act
        string query = "api/userdata?" +
            queryValue
            .Replace("{userId}", registredUser.Id.ToString())
            .Replace("{UserDataId}", response0.Id.ToString());

        using var request = new HttpRequestMessage(HttpMethod.Get, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        // check data
        UserDataDto response = await responseMessage.Content.ReadAsAsync<UserDataDto>();

        Assert.NotNull(response);
        Assert.Equal(response0.Id, response.Id);
        Assert.Equal(data.Title, response.Title);
        Assert.Equal(data.Data, response.Data);
    }

    [Fact]
    public async Task GetUserData_Admin_Returns_Ok()
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
        string query = $"api/userdata?UserId={user.Id}&UserDataId={response0.Id}";
        using var request = new HttpRequestMessage(HttpMethod.Get, query);

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
    }

    [Fact]
    public async Task GetUserData_Unauthorized_Returns_Unauthorized()
    {
        // Act
        using HttpResponseMessage responseMessage = await _client.GetAsync("api/userdata?UserId=1&userDataId=1");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetUserData_NotOwn_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIds.PowerUser.ToString());

        string query = "api/userdata?UserId=1&UserDataId=1";
        using var request = new HttpRequestMessage(HttpMethod.Get, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion

    #region GetUserDataByUserId

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetUserDataByUserId_Own_Returns_Ok(bool useUserId)
    {
        // Arrange
        using RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIds.User.ToString());
        List<UserDataDto> dataList = new List<UserDataDto>();
        const int dataCount = 2;

        for (int i = 0; i < dataCount; i++)
        {
            var data = new AddUserDataDto
            {
                Title = $"Title {i + 1}!",
                Data = $"Data {i + 1}!"
            };

            // add new user data
            UserDataDto response0 = await AddNewUserData(user, data);
        }

        // Act
        string query = "api/userdata/all" +
            (useUserId ? $"?userId={user.Id}" : "");

        using var request = new HttpRequestMessage(HttpMethod.Get, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        // check data
        UserDataDto[] response = await responseMessage.Content.ReadAsAsync<UserDataDto[]>();

        Assert.NotNull(response);
        Assert.Equal(dataCount, response.Length);
    }

    [Fact]
    public async Task GetUserDataByUserId_Admin_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());
        using RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIds.User.ToString());

        List<UserDataDto> dataList = new List<UserDataDto>();
        const int dataCount = 5;

        for (int i = 0; i < dataCount; i++)
        {
            var data = new AddUserDataDto
            {
                Title = $"User Test Title {i + 1}!",
                Data = $"User Test Data {i + 1}!"
            };

            // add new user data
            UserDataDto response0 = await AddNewUserData(user, data);
        }

        // Act
        string query = $"api/userdata/all?UserId={user.Id}";
        using var request = new HttpRequestMessage(HttpMethod.Get, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        // check data
        UserDataDto[] response = await responseMessage.Content.ReadAsAsync<UserDataDto[]>();

        Assert.NotNull(response);
        Assert.Equal(dataCount, response.Length);
    }

    [Fact]
    public async Task GetUserDataByUserId_Unauthorized_Returns_Unauthorized()
    {
        // Act
        using HttpResponseMessage responseMessage = await _client.GetAsync("api/userdata/all?UserId=1");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetUserDataByUserId_NotOwn_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIds.PowerUser.ToString());

        string query = "api/userdata/all?UserId=1";
        using var request = new HttpRequestMessage(HttpMethod.Get, query);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    #endregion
}
