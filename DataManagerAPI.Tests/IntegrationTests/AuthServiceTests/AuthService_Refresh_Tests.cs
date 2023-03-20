using DataManagerAPI.Dto;
using DataManagerAPI.Dto.Constants;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region RefreshToken
    [Fact]
    public async Task RefreshToken_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.User.ToString());

        // Expiration time of JWT token is stored in seconds.
        // If time passed between Login and Refresh is < 1 second,
        // old and new tokens will be equal. Have a break 2 seconds to avoid such case.
        Thread.Sleep(2000); // 2 seconds

        TokenApiModelDto requestData = new TokenApiModelDto
        {
            AccessToken = registredUser.LoginData!.AccessToken,
            RefreshToken = registredUser.LoginData.RefreshToken
        };

        // Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

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
    public async Task RefreshToken_IncorrectAccessToken_Returns_Unauthorized()
    {
        // Arrange
        TokenApiModelDto requestData = new TokenApiModelDto
        {
            AccessToken = "Incorrect",
            RefreshToken = "fake"
        };

        // Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_IncorrectRefreshToken_Returns_Unauthorized()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.User.ToString());

        TokenApiModelDto requestData = new TokenApiModelDto
        {
            AccessToken = registredUser.LoginData!.AccessToken,
            RefreshToken = "fake"
        };

        // Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", requestData);

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
    public async Task RefreshToken_IncorrectRequest_Returns_BadRequest(string accessToken, string refreshToken)
    {
        // Arrange
        TokenApiModelDto? request = null;
        if (accessToken != null || refreshToken != null)
        {
            request = new TokenApiModelDto { AccessToken = accessToken!, RefreshToken = refreshToken };
        }

        // Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/refresh", request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
    }
    #endregion

}
