using DataManagerAPI.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Win32;
using System.Net.Http.Json;
using Xunit;
using static DataManagerAPI.Integration.Tests.TestWebApplicationFactory;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DataManagerAPI.Integration.Tests;

public class RegisterUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public RegisterUserTests(CustomWebApplicationFactory<Program> factory)
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
        RegisterUserDto requestData = DatabaseFixture.GenerateUniqueUserData("Admin");

        //var stringContent = new StringContent(JsonConvert.SerializeObject(requestData), UnicodeEncoding.UTF8, "application/json");

        //Act
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", requestData);

        // Assert
        Assert.NotNull(responseMessage);

        responseMessage.EnsureSuccessStatusCode();

        UserDto response = await responseMessage.Content.ReadAsAsync<UserDto>();
        Assert.NotNull(response);
        Assert.Equal(requestData.FirstName, response.FirstName);
    }

    [Fact]
    public async Task Post_LoginUser_ReturnsPairOfTokens()
    {
        // Arrange

        RegisterUserDto newUser = DatabaseFixture.GenerateUniqueUserData("User");
        HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("api/auth/register", newUser);
        UserDto responseRegister = await responseMessage.Content.ReadAsAsync<UserDto>();

        LoginUserDto requestData = new LoginUserDto
        {
            Login = newUser.Login,
            Password = newUser.Password
        };

        //Act
        responseMessage = await _client.PostAsJsonAsync("api/auth/login", requestData);

        // Assert
        Assert.NotNull(responseMessage);

        responseMessage.EnsureSuccessStatusCode();

        LoginUserResponseDto response = await responseMessage.Content.ReadAsAsync<LoginUserResponseDto>();
        Assert.NotNull(response);
        Assert.NotEmpty(response.Token);
        Assert.NotEmpty(response.RefreshToken);
    }
}
