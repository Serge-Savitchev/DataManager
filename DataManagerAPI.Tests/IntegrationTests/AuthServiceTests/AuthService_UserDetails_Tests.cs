using DataManagerAPI.Dto;
using DataManagerAPI.Dto.Constants;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region GetUserDetails
    [Fact]
    public async Task GetUserDetails_Admin_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.Admin.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/auth/{registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        var response = await responseMessage.Content.ReadAsAsync<UserDetailsDto>();
        Assert.NotNull(response);
        Assert.Equal(registredUser.Id, response.Id);
        Assert.Equal(registredUser.RegisteredUser.LastName, response.LastName);
        Assert.Equal(registredUser.RegisteredUser.Login, response.Login);
    }

    [Fact]
    public async Task GetUserDetails_NotAdmin_Returns_Forbidden()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIdsDto.PowerUser.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/auth/{registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }
    #endregion
}
