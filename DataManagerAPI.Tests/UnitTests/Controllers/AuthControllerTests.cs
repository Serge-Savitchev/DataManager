using DataManagerAPI.Controllers;
using DataManagerAPI.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DataManagerAPI.Tests.UnitTests.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task RegisterUser_NotAdmin_Returns_Forbidden()
    {
        // Arrange
        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        controllerContext.HttpContext.Items["User"] = new CurrentUserDto
        {
            User = new UserDto { Role = "PowerUser" }
        };

        var controller = new AuthController(null!);
        controller.ControllerContext = controllerContext;

        RegisteredUserDto user = new RegisteredUserDto
        {
            Role = "PowerUser"
        };

        // Act
        var response = await controller.RegisterUser(user);

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, (response.Result! as StatusCodeResult)!.StatusCode);
    }

    [Theory]
    [InlineData("Revoke")]
    [InlineData("UpdatePassword")]
    [InlineData("LogOut")]
    [InlineData("RegisteredUser")]
    public async Task OtherMethods_Unauthorized_Returns_Unauthorized(string method)
    {
        // Arrange
        var controller = new AuthController(null!);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        StatusCodeResult response = null!;

        switch (method)
        {
            case "Revoke":
                response = (await controller.Revoke() as StatusCodeResult)!;
                break;
            case "UpdatePassword":
                response = (await controller.UpdatePassword(null!) as StatusCodeResult)!;
                break;
            case "LogOut":
                response = (controller.LogOut() as StatusCodeResult)!;
                break;
            case "RegisteredUser":
                var result = await controller.RegisterUser(new RegisteredUserDto { Role = "Admin" });
                response = (result.Result as StatusCodeResult)!;
                break;
        }

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, response?.StatusCode);
    }
}
