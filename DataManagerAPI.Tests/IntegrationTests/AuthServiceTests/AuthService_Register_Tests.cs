using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.AuthServiceTests;

public partial class AuthServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region Register
    [Fact]
    public async Task Post_RedisterUser_Returns_NewUser()
    {
        // Arrange
        using RegisterUserTestData requestData = UsersForTestsHelper.GenerateUniqueUserData(RoleIds.Admin.ToString());

        //Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", requestData.UserData);

        // Assert
        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.NotNull(response);
        Assert.Equal(requestData.UserData!.FirstName, response.FirstName);

        requestData.Id = response.Id;
    }

    [Fact]
    public async Task Post_RedisterUser_UserExists_Returns_Conflict()
    {
        // Arrange
        using RegisterUserTestData registredUser = await UsersForTestsHelper.FindOrCreateRegistredUser(_client, RoleIds.Admin.ToString());

        //Act
        using HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", registredUser.UserData);

        // Assert
        Assert.Equal(StatusCodes.Status409Conflict, (int)responseMessage.StatusCode);
    }
    #endregion
}
