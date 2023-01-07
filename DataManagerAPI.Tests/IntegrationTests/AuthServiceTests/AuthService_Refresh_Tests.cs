using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Xunit;
using static DataManagerAPI.Tests.IntegrationTests.TestWebApplicationFactory;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region Refresh
    [Fact]
    public async Task Post_RefreshToken_ReturnsNewPairOfTokens()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());

        // Expiration time of JWT token is stored in seconds.
        // If time passed between Login and Refresh is < 1 second,
        // old and new tokens will be equal. Have a break 2 seconds to avoid such case.
        Thread.Sleep(2000); // 2 seconds

        TokenApiModelDto requestData = new TokenApiModelDto
        {
            AccessToken = registredUser.LoginData!.AccessToken,
            RefreshToken = registredUser.LoginData.RefreshToken
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        TokenApiModelDto response = await responseMessage.Content.ReadAsAsync<TokenApiModelDto>();
        Assert.NotNull(response);
        Assert.NotEmpty(response.AccessToken);
        Assert.NotEmpty(response.RefreshToken);
        Assert.NotEqual(requestData.AccessToken, response.AccessToken);
        Assert.NotEqual(requestData.RefreshToken, response.RefreshToken);

        registredUser.LoginData = new LoginUserResponseDto
        {
            AccessToken = response.AccessToken,
            RefreshToken = response.RefreshToken
        };
    }

    [Fact]
    public async Task Post_RefreshToken_IncorrectAccessToken_ReturnsUnauthorized()
    {
        // Arrange
        TokenApiModelDto requestData = new TokenApiModelDto
        {
            AccessToken = "Incorrect",
            RefreshToken = "fake"
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task Post_RefreshToken_IncorrectRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.User.ToString());

        TokenApiModelDto requestData = new TokenApiModelDto
        {
            AccessToken = registredUser.LoginData!.AccessToken,
            RefreshToken = "fake"
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

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
    public async Task Post_RefreshToken_IncorrectRequest_ReturnsBadRequest(string accessToken, string refreshToken)
    {
        // Arrange
        TokenApiModelDto? request = null;
        if (accessToken != null || refreshToken != null)
        {
            request = new TokenApiModelDto { AccessToken = accessToken!, RefreshToken = refreshToken };
        }

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }
    #endregion

}
