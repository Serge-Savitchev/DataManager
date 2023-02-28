using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region ChangePassword
    [Fact]
    public async Task ChangePassword_User_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.User.ToString());

        string newPassword = "newPassword";

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, "api/auth/changepassword");
        request.Content = new StringContent(JsonConvert.SerializeObject(newPassword), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        registredUser.RegisterUser.Password = newPassword;

        // check login with new password
        LoginUserDto requestData = new LoginUserDto
        {
            Login = registredUser.RegisterUser.Login,
            Password = newPassword
        };

        using var responseMessage0 = await _client.PostAsJsonAsync("api/auth/login", requestData);
        responseMessage0.EnsureSuccessStatusCode();

        LoginUserResponseDto response = await responseMessage0.Content.ReadAsAsync<LoginUserResponseDto>();
        Assert.NotNull(response);
        Assert.NotEmpty(response.AccessToken);
        Assert.NotEmpty(response.RefreshToken);

        registredUser.LoginData = response;
    }

    [Fact]
    public async Task ChangePassword_Admin_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());
        using RegisteredUserTestData userToChange = await UsersForTestsHelper.FindOrCreateRegisteredUser(_client, RoleIds.User.ToString());
        string newPassword = "newPassword";

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/auth/changepassword/{userToChange.Id}");
        request.Content = new StringContent(JsonConvert.SerializeObject(newPassword), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();
        userToChange.RegisterUser.Password = newPassword;

        // check login with new password
        LoginUserDto requestData = new LoginUserDto
        {
            Login = userToChange.RegisterUser.Login,
            Password = newPassword
        };

        using var responseMessage0 = await _client.PostAsJsonAsync("api/auth/login", requestData);
        responseMessage0.EnsureSuccessStatusCode();

        LoginUserResponseDto response = await responseMessage0.Content.ReadAsAsync<LoginUserResponseDto>();
        Assert.NotNull(response);
        Assert.NotEmpty(response.AccessToken);
        Assert.NotEmpty(response.RefreshToken);

        userToChange.LoginData = response;
    }

    [Fact]
    public async Task ChangePassword_NotAdmin_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());
        using RegisteredUserTestData userToChange = await UsersForTestsHelper.FindOrCreateRegisteredUser(_client, RoleIds.User.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Put, $"api/auth/changepassword/{userToChange.Id}");
        request.Content = new StringContent(JsonConvert.SerializeObject("newPassword"), Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }
    #endregion
}
