using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Xunit;
using static DataManagerAPI.Tests.IntegrationTests.TestWebApplicationFactory;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region Login
    [Fact]
    public async Task Post_LoginUser_ReturnsPairOfTokens()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.User.ToString());

        LoginUserDto requestData = new LoginUserDto
        {
            Login = registredUser.UserData.Login,
            Password = registredUser.UserData.Password
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
        Assert.NotNull(response);
        Assert.NotEmpty(response.AccessToken);
        Assert.NotEmpty(response.RefreshToken);

        registredUser.LoginData = response;
    }

    [Fact]
    public async Task Post_LoginUser_IncorrectLogin_ReturnsNotFound()
    {
        // Arrange
        LoginUserDto requestData = new LoginUserDto
        {
            Login = "Incorrect",
            Password = "fake"
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Post_LoginUser_IncorrectPassword_ReturnsUnauthorized()
    {
        // Arrange
        RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.User.ToString());

        LoginUserDto requestData = new LoginUserDto
        {
            Login = registredUser.UserData.Login,
            Password = $"{registredUser.UserData.Password}Incorrect"
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

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
    public async Task Post_LoginUser_IncorrectRequest_ReturnsBadRequest(string login, string password)
    {
        // Arrange
        LoginUserDto? request = null;

        if (login != null || password != null)
        {
            request = new LoginUserDto { Login = login!, Password = password };
        }

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }
    #endregion
}
