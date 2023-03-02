using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region Login
    [Fact]
    public async Task LoginUser_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateRegisteredUser(_client, RoleIds.User.ToString());

        LoginUserDto requestData = new LoginUserDto
        {
            Login = registredUser.RegisteredUser.Login,
            Password = registredUser.RegisteredUser.Password
        };

        // Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
        Assert.NotNull(response);
        Assert.NotEmpty(response.AccessToken);
        Assert.NotEmpty(response.RefreshToken);

        registredUser.LoginData = response;
    }

    [Fact]
    public async Task LoginUser_IncorrectLogin_Returns_NotFound()
    {
        // Arrange
        LoginUserDto requestData = new LoginUserDto
        {
            Login = "Incorrect",
            Password = "fake"
        };

        // Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task LoginUser_IncorrectPassword_Returns_Unauthorized()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateRegisteredUser(_client, RoleIds.User.ToString());

        LoginUserDto requestData = new LoginUserDto
        {
            Login = registredUser.RegisteredUser.Login,
            Password = $"{registredUser.RegisteredUser.Password}Incorrect"
        };

        // Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "fake")]
    [InlineData("fake", null)]
    [InlineData("", "fake")]
    [InlineData("fake", "")]
    [InlineData("", "")]
    public async Task LoginUser_IncorrectRequest_Returns_BadRequest(string login, string password)
    {
        // Arrange
        LoginUserDto? request = null;

        if (login != null || password != null)
        {
            request = new LoginUserDto { Login = login!, Password = password };
        }

        // Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }
    #endregion
}
