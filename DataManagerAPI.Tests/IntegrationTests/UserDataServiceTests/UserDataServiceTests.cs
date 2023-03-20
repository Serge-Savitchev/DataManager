using DataManagerAPI.Dto;
using DataManagerAPI.Dto.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.UserDataServiceTests;

public partial class UserDataServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserDataServiceTests(CustomWebApplicationFactory<Program> factory)
    {
        DatabaseFixture.PrepareDatabase(factory);

        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    #region AddUserData

    [Fact]
    public async Task AddUserData_Unauthorized_Returns_Unauthorized()
    {
        // Act
        var data = new AddUserDataDto
        {
            Title = "Test Title",
            Data = "Test Data"
        };

        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/userdata/1", data);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task AddUserData_BadRequest_Returns_BadRequest()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.Admin.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/userdata/{registredUser.Id}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(2), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task AddUserData_NotOwn_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.PowerUser.ToString());

        // Act
        var data = new AddUserDataDto
        {
            Title = "Test Title",
            Data = "Test Data"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/userdata/1")
        {
            Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task AddUserData_Own_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.PowerUser.ToString());

        // Act
        var data = new AddUserDataDto
        {
            Title = "Test Title",
            Data = "Test Data"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/userdata/{registredUser.Id}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        // check data
        UserDataDto response = await responseMessage.Content.ReadAsAsync<UserDataDto>();

        Assert.NotNull(response);
        Assert.Equal(data.Title, response.Title);
        Assert.Equal(data.Data, response.Data);
    }

    [Fact]
    public async Task AddUserData_AdminNotOwn_ReturnsOk()
    {
        // Arrange
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.Admin.ToString());
        using RegisteredUserTestData user = await UsersForTestsHelper.FindOrCreateRegisteredUser(_client, RoleIdsDto.User.ToString());

        // Act
        var data = new AddUserDataDto
        {
            Title = "Admin's Test Title",
            Data = "Admin's Test Data"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/userdata/{user.Id}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDataDto response = await responseMessage.Content.ReadAsAsync<UserDataDto>();

        // check data
        Assert.NotNull(response);
        Assert.Equal(data.Title, response.Title);
        Assert.Equal(data.Data, response.Data);
    }

    #endregion

    #region UpdateUserData

    [Fact]
    public async Task UpdateUserData_Unauthorized_Returns_Unauthorized()
    {
        // Act
        var data = new AddUserDataDto
        {
            Title = "Test Title",
            Data = "Test Data"
        };

        using HttpResponseMessage responseMessage = await _client.PutAsJsonAsync("api/userdata/1/1", data);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task UpdateUserData_BadRequest_Returns_BadRequest()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.Admin.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/userdata/{registredUser.Id}/1")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new AddUserDataDto { Data = "" }), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task UpdateUserData_NotOwn_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.PowerUser.ToString());

        // Act
        var data = new AddUserDataDto
        {
            Title = "Test Title",
            Data = "Test Data"
        };

        using var request = new HttpRequestMessage(HttpMethod.Put, "api/userdata/1/1")
        {
            Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task UpdateUserData_Own_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIdsDto.User.ToString());

        // add new user data
        UserDataDto response0 = await AddNewUserData(registredUser,
            new AddUserDataDto
            {
                Title = "User Test Title",
                Data = "User Test Data"
            });

        // Act
        var newData = new AddUserDataDto
        {
            Title = "New User Test Title",
            Data = "New User Test Data"
        };

        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/userdata/{registredUser.Id}/{response0.Id}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(newData), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        // check data
        UserDataDto response = await responseMessage.Content.ReadAsAsync<UserDataDto>();

        Assert.NotNull(response);
        Assert.Equal(response0.Id, response.Id);
        Assert.Equal(newData.Title, response.Title);
        Assert.Equal(newData.Data, response.Data);
    }

    [Fact]
    public async Task UpdateUserData_AdminNotOwn_ReturnsOk()
    {
        // Arrange
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.Admin.ToString());
        using RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, RoleIdsDto.User.ToString());

        // add new user data

        UserDataDto response0 = await AddNewUserData(user,
            new AddUserDataDto
            {
                Title = "User's Test Title",
                Data = "User's Test Data"
            });

        // Act
        var newData = new AddUserDataDto
        {
            Title = "Admin's Test Title",
            Data = "Admin's Test Data"
        };

        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/userdata/{user.Id}/{response0.Id}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(newData), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDataDto response = await responseMessage.Content.ReadAsAsync<UserDataDto>();

        // check data
        Assert.NotNull(response);
        Assert.Equal(response0.Id, response.Id);
        Assert.Equal(newData.Title, response.Title);
        Assert.Equal(newData.Data, response.Data);
    }

    #endregion

    #region Helpers

    private async Task<UserDataDto> AddNewUserData(RegisteredUserTestData user, AddUserDataDto data)
    {
        UserDataDto? response0 = null;
        using (var request0 = new HttpRequestMessage(HttpMethod.Post, $"api/userdata/{user.Id}"))
        {
            request0.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            request0.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
            using HttpResponseMessage responseMessage0 = await _client.SendAsync(request0);
            responseMessage0.EnsureSuccessStatusCode();
            response0 = await responseMessage0.Content.ReadAsAsync<UserDataDto>();
            Assert.NotNull(response0);
        }
        return response0;
    }

    #endregion
}
