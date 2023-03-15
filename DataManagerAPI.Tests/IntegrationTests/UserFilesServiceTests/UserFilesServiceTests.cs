using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace DataManagerAPI.Tests.IntegrationTests.UserFilesServiceTests;

public partial class UserFilesServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private const int _defaultBufferSize = 1024 * 4;

    public UserFilesServiceTests(CustomWebApplicationFactory<Program> factory)
    {
        DatabaseFixture.PrepareDatabase(factory);

        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    #region DownloadFile

    [Theory]
    [InlineData("smallFileForDownload.bin", 1024, false)]
    [InlineData("bigFileForDownload.bin", 1024 * 1024, true)]
    public async Task DownloadFile_Returns_Ok(string name, int size, bool bigFile)
    {
        (RegisteredUserTestData User, UserDataDto UserData, UserFileDto UserFile) uploadedFile =
            await UploadFile(RoleIds.User, name, size, bigFile);
        try
        {
            // Act
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/userfiles/{uploadedFile.UserData.Id}/{uploadedFile.UserFile.Id}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", uploadedFile.User.LoginData!.AccessToken);

            using HttpResponseMessage responseMessage = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            // Assert
            responseMessage.EnsureSuccessStatusCode();

            string downLoadedfileName = responseMessage.Content.Headers!.ContentDisposition!.FileNameStar!;

            await using var streamToReadFrom = await responseMessage.Content.ReadAsStreamAsync();
            await using var outputStream = new MemoryStream();
            await using BufferedStream bufferedStream = new(streamToReadFrom, _defaultBufferSize);

            bufferedStream.CopyTo(outputStream);

            Assert.Equal(name, downLoadedfileName);
            Assert.Equal(size, outputStream.Length);
        }
        finally
        {
            uploadedFile.User?.Dispose();
        }
    }

    [Fact]
    public async Task DownloadFile_IncorrectFileId_Returns_NotFound()
    {
        (RegisteredUserTestData User, UserDataDto UserData, UserFileDto UserFile) uploadedFile =
            await UploadFile(RoleIds.User, "file", 1024, false);

        try
        {
            // Act
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/userfiles/{uploadedFile.UserData.Id}/{uploadedFile.UserFile.Id + 20000}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", uploadedFile.User.LoginData!.AccessToken);

            using HttpResponseMessage responseMessage = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, (int)responseMessage.StatusCode);
        }
        finally
        {
            uploadedFile.User?.Dispose();
        }
    }

    #endregion

    #region UploadFile

    [Theory]
    [InlineData("smallFile.bin", 1024, false)]
    [InlineData("bigFile.bin", 1024 * 1024, true)]
    public async Task UploadFile_Returns_Ok(string name, int size, bool bigFile)
    {
        // Act
        (RegisteredUserTestData User, UserDataDto UserData, UserFileDto UserFile) uploadedFile =
            await UploadFile(RoleIds.User, name, size, bigFile);

        try
        {
            // Assert
            Assert.NotNull(uploadedFile.UserFile);
            Assert.Equal(name, uploadedFile.UserFile.Name);
            Assert.Equal(size, uploadedFile.UserFile.Size);
        }
        finally
        {
            uploadedFile.User?.Dispose();
        }
    }

    [Fact]
    public async Task UploadFile_Overwrite_Existing_Returns_Ok()
    {
        // Arrange
        const string name1 = "original.bin", name2 = "new.bin";
        const int size1 = 1024, size2 = 1024 * 2;

        (RegisteredUserTestData User, UserDataDto UserData, UserFileDto UserFile) uploadedFile =
            await UploadFile(RoleIds.User, name1, size1, false);

        try
        {
            using var multipartContent = new MultipartFormDataContent();

            byte[] byteContent = Enumerable.Repeat((byte)0xBB, size2).ToArray();

            using var outStream = new MemoryStream(byteContent);

            using var fileContent = new StreamContent(outStream, _defaultBufferSize);

            multipartContent.Add(fileContent, "multipart/form-data", name2);

            using var request = new HttpRequestMessage(HttpMethod.Post,
                $"api/userfiles/{uploadedFile.UserData.Id}/{uploadedFile.UserFile.Id}?bigFile=auto")
            {
                Content = multipartContent
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", uploadedFile.User.LoginData!.AccessToken);

            // Act
            using HttpResponseMessage responseMessage = await _client.SendAsync(request);

            responseMessage.EnsureSuccessStatusCode();
            UserFileDto response = await responseMessage.Content.ReadAsAsync<UserFileDto>();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(name2, response.Name);
            Assert.Equal(size2, response.Size);
            Assert.Equal(uploadedFile.UserFile.Id, response.Id);
        }
        finally
        {
            uploadedFile.User?.Dispose();
        }
    }

    [Theory]
    [InlineData(StatusCodes.Status403Forbidden)]    // expected status code
    [InlineData(StatusCodes.Status400BadRequest)]   // expected status code
    public async Task UploadFile_Incorrect_UserDataId_Returns_StatusCode(int statusCode)
    {
        // Arrange
        (RegisteredUserTestData User, UserDataDto UserData) newUserData = await CreateUserData(RoleIds.User);
        (RegisteredUserTestData User, UserDataDto UserData) otherUserData = await CreateUserData(RoleIds.ReadOnlyUser);

        try
        {
            using var multipartContent = new MultipartFormDataContent();

            byte[] byteContent = Enumerable.Repeat((byte)0xBB, 10).ToArray();

            using var outStream = new MemoryStream(byteContent);

            using var fileContent = new StreamContent(outStream, _defaultBufferSize);

            multipartContent.Add(fileContent, "multipart/form-data", "fake.bin");

            // set userData Id in accordance with expected status code
            int userDataId = statusCode == StatusCodes.Status403Forbidden ?
                otherUserData.UserData.Id :     // real existing user data. expect Status403Forbidden
                200000;                         // non-existent user data. expect Status400BadRequest

            using var request = new HttpRequestMessage(HttpMethod.Post,
                $"api/userfiles/{userDataId}/0?bigFile=false")
            {
                Content = multipartContent
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newUserData.User.LoginData!.AccessToken);

            // Act
            using HttpResponseMessage responseMessage = await _client.SendAsync(request);

            // Assert
            Assert.Equal(statusCode, (int)responseMessage.StatusCode);
        }
        finally
        {
            newUserData.User?.Dispose();
            otherUserData.User?.Dispose();
        }
    }
    #endregion

    #region Helpers

    private async Task<(RegisteredUserTestData User, UserDataDto UserData, UserFileDto UserFile)>
        UploadFile(RoleIds role, string fileName, int fileSize, bool bigFile = false)
    {
        (RegisteredUserTestData User, UserDataDto UserData) userData = await CreateUserData(role);

        UserFileDto response = await UploadFile(userData, fileName, fileSize, bigFile);
        return (userData.User, userData.UserData, response);
    }

    private async Task<UserFileDto>
        UploadFile((RegisteredUserTestData User, UserDataDto UserData) userData, string fileName, int fileSize, bool bigFile = false)
    {
        // prepare form data
        using var multipartContent = new MultipartFormDataContent();

        byte[] byteContent = Enumerable.Repeat((byte)0xAA, fileSize).ToArray();

        using var outStream = new MemoryStream(byteContent);

        using var fileContent = new StreamContent(outStream, _defaultBufferSize);

        multipartContent.Add(fileContent, "multipart/form-data", fileName);

        using var request = new HttpRequestMessage(HttpMethod.Post,
            $"api/userfiles/{userData.UserData.Id}/0?bigFile={bigFile}")
        {
            Content = multipartContent
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userData.User.LoginData!.AccessToken);

        using HttpResponseMessage responseMessage = await _client.SendAsync(request);

        responseMessage.EnsureSuccessStatusCode();

        UserFileDto response = await responseMessage.Content.ReadAsAsync<UserFileDto>();
        return response;
    }

    private async Task<(RegisteredUserTestData User, UserDataDto UserData)> CreateUserData(RoleIds role)
    {
        RegisteredUserTestData user = await UsersForTestsHelper.CreateNewLoggedInUser(_client, role.ToString());

        // add new user data
        UserDataDto? response0 = null;
        using (var request0 = new HttpRequestMessage(HttpMethod.Post, $"api/userdata/{user.Id}"))
        {
            var data = new AddUserDataDto
            {
                Title = "User Test Title",
                Data = "User Test Data"
            };

            request0.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            request0.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.LoginData!.AccessToken);
            using HttpResponseMessage responseMessage0 = await _client.SendAsync(request0);
            response0 = await responseMessage0.Content.ReadAsAsync<UserDataDto>();
        }

        return (user, response0);
    }

    #endregion
}
