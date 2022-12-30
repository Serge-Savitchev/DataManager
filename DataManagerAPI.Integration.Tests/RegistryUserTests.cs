using DataManagerAPI.Dto;
using DataManagerAPI.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;
using static DataManagerAPI.Integration.Tests.TestWebApplicationFactory;

namespace DataManagerAPI.Integration.Tests;

public class RegistryUserTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program>
        _factory;

    public RegistryUserTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<UsersDBContext>();
            db.Database.Migrate();
        }


        //var db = _factory.Services.GetService(typeof(UsersDBContext));
        //var db = services.SingleOrDefault(d => d.ServiceType == typeof(UsersDBContext));

        //db.Database.Migrate();

    }

    [Fact]
    public async Task Post_DeleteAllMessagesHandler_ReturnsRedirectToRoot()
    {
        // Arrange
        RegisterUserDto requestData = new RegisterUserDto
        {
            FirstName = "FirstName",
            LastName = "LastName",
            Email = "a@a.com",
            Login = "Login",
            Password = "Password",
            Role = "Admin"
        };

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

}
