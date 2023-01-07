using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Xunit;
using static DataManagerAPI.Tests.IntegrationTests.TestWebApplicationFactory;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region UserDetails
    [Fact]
    public async Task Get_UserDetails_Admin_ReturnsOk()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.Admin.ToString());

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/auth/{registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        var response = await responseMessage.Content.ReadAsAsync<UserDetailsDto>();
        Assert.NotNull(response);
        Assert.Equal(registredUser.Id, response.Id);
        Assert.Equal(registredUser.UserData.LastName, response.LastName);
        Assert.Equal(registredUser.UserData.Login, response.Login);
    }

    [Fact]
    public async Task Get_UserDetails_NotAdmin_ReturnsForbidden()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedUser(_client, RoleIds.PowerUser.ToString());

        //Act
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/auth/{registredUser.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);

        HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
    }
    #endregion
}
