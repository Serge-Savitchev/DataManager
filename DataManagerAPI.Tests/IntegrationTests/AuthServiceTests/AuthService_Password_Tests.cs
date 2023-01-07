using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using static DataManagerAPI.Tests.IntegrationTests.TestWebApplicationFactory;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
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
}
