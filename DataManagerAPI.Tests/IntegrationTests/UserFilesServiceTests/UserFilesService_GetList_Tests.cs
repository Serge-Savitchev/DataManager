using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.UserFilesServiceTests;

public partial class UserFilesServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    #region GetList

    [Fact]
    public async Task GetList_Returns_Ok()
    {
        // Arrange
        (RegisteredUserTestData User, UserDataDto UserData) newUserData = await CreateUserData(RoleIds.User);

        try
        {
            const int count = 5;

            List<UserFileDto> files = new();

            for (int i = 0; i < count; i++)
            {
                var file = await UploadFile(newUserData, $"file-{i}.bin", 512 * (i + 1), i % 2 == 0);
                files.Add(file);
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/userfiles/{newUserData.UserData.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newUserData.User.LoginData!.AccessToken);

            // Act
            using HttpResponseMessage responseMessage = await _client.SendAsync(request);

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            UserFileDto[] response = await responseMessage.Content.ReadAsAsync<UserFileDto[]>();

            Assert.Equal(count, response.Length);

            for (int i = 0; i < count; i++)
            {
                var userFile = response.FirstOrDefault(x => x.Id == files[i].Id);
                Assert.NotNull(userFile);
                Assert.Equal(files[i].Size, userFile.Size);
                Assert.Equal(files[i].Name, userFile.Name);
            }
        }
        finally
        {
            newUserData.User?.Dispose();
        }
    }

    [Fact]
    public async Task GetList_Unauthorized_Returns_Unauthorized()
    {
        // Act
        using HttpResponseMessage responseMessage = await _client.GetAsync("api/userfiles/1");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task GetList_NotOwn_Returns_Forbidden()
    {
        // Arrange
        (RegisteredUserTestData User, UserDataDto UserData) newUserData = await CreateUserData(RoleIds.User);

        using RegisteredUserTestData powerUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());

        try
        {
            const int count = 3;

            List<UserFileDto> files = new();

            for (int i = 0; i < count; i++)
            {
                var file = await UploadFile(newUserData, $"file-{i}.bin", 512 * (i + 1), i % 2 == 0);
                files.Add(file);
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/userfiles/{newUserData.UserData.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", powerUser.LoginData!.AccessToken);

            // Act
            using HttpResponseMessage responseMessage = await _client.SendAsync(request);

            // Assert
            Assert.Equal(StatusCodes.Status403Forbidden, (int)responseMessage.StatusCode);
        }
        finally
        {
            newUserData.User?.Dispose();
        }
    }

    [Fact]
    public async Task GetList_Admin_Returns_Ok()
    {
        // Arrange
        (RegisteredUserTestData User, UserDataDto UserData) newUserData = await CreateUserData(RoleIds.User);

        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        try
        {
            const int count = 7;

            List<UserFileDto> files = new();

            for (int i = 0; i < count; i++)
            {
                var file = await UploadFile(newUserData, $"file-{i}.bin", 512 * (i + 1), i % 2 == 0);
                files.Add(file);
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/userfiles/{newUserData.UserData.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);

            // Act
            using HttpResponseMessage responseMessage = await _client.SendAsync(request);

            // Assert
            responseMessage.EnsureSuccessStatusCode();
            UserFileDto[] response = await responseMessage.Content.ReadAsAsync<UserFileDto[]>();

            Assert.Equal(count, response.Length);

            for (int i = 0; i < count; i++)
            {
                var userFile = response.FirstOrDefault(x => x.Id == files[i].Id);
                Assert.NotNull(userFile);
                Assert.Equal(files[i].Size, userFile.Size);
                Assert.Equal(files[i].Name, userFile.Name);
            }
        }
        finally
        {
            newUserData.User?.Dispose();
        }
    }

    [Fact]
    public async Task GetList_IncorrectId_Returns_BadRequest()
    {
        // Arrange
        (RegisteredUserTestData User, UserDataDto UserData) newUserData = await CreateUserData(RoleIds.User);

        try
        {
            const int count = 1;

            List<UserFileDto> files = new();

            for (int i = 0; i < count; i++)
            {
                var file = await UploadFile(newUserData, $"file-{i}.bin", 512 * (i + 1), i % 2 == 0);
                files.Add(file);
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, "api/userfiles/20000");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newUserData.User.LoginData!.AccessToken);

            // Act
            using HttpResponseMessage responseMessage = await _client.SendAsync(request);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, (int)responseMessage.StatusCode);
        }
        finally
        {
            newUserData.User?.Dispose();
        }
    }

    #endregion
}
