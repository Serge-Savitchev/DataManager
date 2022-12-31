using DataManagerAPI.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;
using static DataManagerAPI.Integration.Tests.TestWebApplicationFactory;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DataManagerAPI.Integration.Tests;

public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthorizationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        DatabaseFixture.PrepareDatabase(_factory);
    }

    [Fact]
    public async Task Post_RedisterUser_Returns_NewUser()
    {
        // Arrange
        RegisterUserTestData requestData = DatabaseFixture.GenerateUniqueUserData("Admin");

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", requestData.UserData);

        // Assert
        Assert.NotNull(responseMessage);

        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.NotNull(response);
        Assert.Equal(requestData.UserData!.FirstName, response.FirstName);

        requestData.Id = response.Id;
        requestData.Locked = false;
    }

    [Fact]
    public async Task Post_RedisterUser_Returns_ConflictCode()
    {
        // Arrange
        var registredUser = await FindOrCreateRegistredUser("admin");

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", registredUser.UserData);

        // Assert
        Assert.NotNull(responseMessage);
        Assert.Equal(StatusCodes.Status409Conflict, (int)responseMessage.StatusCode);

        registredUser.Locked = false;
    }

    [Fact]
    public async Task Post_LoginUser_ReturnsPairOfTokens()
    {
        // Arrange
        var registredUser = await FindOrCreateRegistredUser("user");

        LoginUserDto requestData = new LoginUserDto
        {
            Login = registredUser.UserData.Login,
            Password = registredUser.UserData.Password
        };

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

        // Assert
        Assert.NotNull(responseMessage);

        responseMessage.EnsureSuccessStatusCode();

        LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
        Assert.NotNull(response);
        Assert.NotEmpty(response.Token);
        Assert.NotEmpty(response.RefreshToken);

        registredUser.LoginData = response;
        registredUser.Locked = false;
    }

    private async Task<RegisterUserTestData> FindOrCreateRegistredUser(string role)
    {
        var registredUser = DatabaseFixture
            .FindRegisterUser(x => x.LoginData == null && !x.Locked && role.Equals(x.UserData.Role, StringComparison.InvariantCultureIgnoreCase));

        if (registredUser == null)
        {
            registredUser = DatabaseFixture.GenerateUniqueUserData(role);
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", registredUser.UserData);
            UserDto user = await responseMessage.Content.ReadAsAsync<UserDto>();
            registredUser.Id = user.Id;
        }

        return registredUser;
    }

    private async Task<RegisterUserTestData> FindOrCreateLoggedUser(string role)
    {
        var registredUser = DatabaseFixture
            .FindRegisterUser(x => x.LoginData != null && !x.Locked && role.Equals(x.UserData.Role, StringComparison.InvariantCultureIgnoreCase));

        if (registredUser == null)
        {
            registredUser = DatabaseFixture.GenerateUniqueUserData(role);
            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", registredUser.UserData);
            UserDto user = await responseMessage.Content.ReadAsAsync<UserDto>();
            registredUser.Id = user.Id;

            LoginUserDto requestData = new LoginUserDto
            {
                Login = registredUser.UserData.Login,
                Password = registredUser.UserData.Password
            };

            responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);
            LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
            registredUser.LoginData = response;
        }

        return registredUser;
    }

}
