using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.UserFilesServiceTests;

public partial class UserFilesServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Fact]
    public async Task DeleteFile_Returns_Ok()
    {
        // Arrange
        (RegisteredUserTestData User, UserDataDto UserData) newUserData = await CreateUserData(RoleIds.User);

        try
        {
            const int count = 4;

            List<UserFileDto> files = new();

            for (int i = 0; i < count; i++)
            {
                var file = await UploadFile(newUserData, $"fileToDelete-{i}.bin", 1024 * (i + 1), i % 2 == 0);
                files.Add(file);
            }

            // request for file deleting
            using var request0 = new HttpRequestMessage(HttpMethod.Delete, $"api/userfiles?userDataId={newUserData.UserData.Id}&fileId={files[0].Id}");
            request0.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newUserData.User.LoginData!.AccessToken);

            // request for file deleting
            using var request1 = new HttpRequestMessage(HttpMethod.Delete, $"api/userfiles?userDataId={newUserData.UserData.Id}&fileId={files[1].Id}");
            request1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newUserData.User.LoginData!.AccessToken);

            // Act
            // delete file
            using HttpResponseMessage responseMessage0 = await _client.SendAsync(request0);
            // delete file
            using HttpResponseMessage responseMessage1 = await _client.SendAsync(request1);

            // Assert
            responseMessage0.EnsureSuccessStatusCode();
            responseMessage1.EnsureSuccessStatusCode();

            // Get list of files
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/userfiles?userDataId={newUserData.UserData.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newUserData.User.LoginData!.AccessToken);
            using HttpResponseMessage responseMessage = await _client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();

            UserFileDto[] response = await responseMessage.Content.ReadAsAsync<UserFileDto[]>();

            Assert.Equal(count - 2, response.Length);

            // check that deleted files absent in the response
            Assert.Null(response.FirstOrDefault(x => x.Id == files[0].Id));
            Assert.Null(response.FirstOrDefault(x => x.Id == files[1].Id));
        }
        finally
        {
            newUserData.User?.Dispose();
        }
    }

    [Fact]
    public async Task DeleteFile_Unauthorized_Returns_Unauthorized()
    {
        // Act
        using HttpResponseMessage responseMessage = await _client.DeleteAsync("api/userfiles?fileId=1&userDataId=1");

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, (int)responseMessage.StatusCode);
    }

    [Fact]
    public async Task DeleteFile_NotOwn_Returns_Forbidden()
    {
        // Arrange
        (RegisteredUserTestData User, UserDataDto UserData) newUserData = await CreateUserData(RoleIds.User);

        using RegisteredUserTestData powerUser = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.PowerUser.ToString());

        try
        {
            var file = await UploadFile(newUserData, $"fileToDelete.bin", 512, false);

            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/userfiles?userDataId={newUserData.UserData.Id}");
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
    public async Task DeleteFile_Admin_Returns_Ok()
    {
        // Arrange
        (RegisteredUserTestData User, UserDataDto UserData) newUserData = await CreateUserData(RoleIds.User);
        using RegisteredUserTestData admin = await UsersForTestsHelper.FindOrCreateLoggedInUser(_client, RoleIds.Admin.ToString());

        try
        {
            const int count = 2;

            List<UserFileDto> files = new();

            for (int i = 0; i < count; i++)
            {
                var file = await UploadFile(newUserData, $"fileToDeleteByAdmin-{i}.bin", 1024, i % 2 == 0);
                files.Add(file);
            }

            // request for file deleting
            using var request0 = new HttpRequestMessage(HttpMethod.Delete, $"api/userfiles?userDataId={newUserData.UserData.Id}&fileId={files[0].Id}");
            request0.Headers.Authorization = new AuthenticationHeaderValue("Bearer", admin.LoginData!.AccessToken);

            // Act
            // delete file
            using HttpResponseMessage responseMessage0 = await _client.SendAsync(request0);

            // Assert
            responseMessage0.EnsureSuccessStatusCode();

            // Get list of files
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/userfiles?userDataId={newUserData.UserData.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newUserData.User.LoginData!.AccessToken);
            using HttpResponseMessage responseMessage = await _client.SendAsync(request);
            responseMessage.EnsureSuccessStatusCode();

            UserFileDto[] response = await responseMessage.Content.ReadAsAsync<UserFileDto[]>();

            Assert.Single(response);

            // check that deleted file absences in the response
            Assert.Null(response.FirstOrDefault(x => x.Id == files[0].Id));
        }
        finally
        {
            newUserData.User?.Dispose();
        }
    }
}
