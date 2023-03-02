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
    #region Register
    [Fact]
    public async Task RedisterUser_Returns_Ok()
    {
        // Arrange
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());
        using RegisteredUserTestData requestData = UsersForTestsHelper.GenerateUniqueUserData(RoleIds.Admin.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/register")
        {
            Content = new StringContent(JsonConvert.SerializeObject(requestData.RegisteredUser), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.NotNull(response);
        Assert.Equal(requestData.RegisteredUser!.FirstName, response.FirstName);

        requestData.Id = response.Id;
    }

    [Fact]
    public async Task RedisterUser_UserExists_Returns_Conflict()
    {
        // Arrange
        using RegisteredUserTestData registredUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        // Act
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/register")
        {
            Content = new StringContent(JsonConvert.SerializeObject(registredUser.RegisteredUser), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registredUser.LoginData!.AccessToken);
        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, (int)responseMessage.StatusCode);
    }
    #endregion
}
