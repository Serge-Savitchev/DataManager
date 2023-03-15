using DataManagerAPI.Controllers;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataManagerAPI.Tests.UnitTests.Controllers;

public class UserFilesControllerTests
{
    [Fact]
    public async Task DeleteFile_Incorrect_User_Returns_Forbidden()
    {
        // Arrange
        var userFilesService = new Mock<IUserFilesService>();
        userFilesService.Setup(x => x.DeleteFile(
            It.IsAny<CurrentUserDto?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<int> { StatusCode = ResultStatusCodes.Status403Forbidden });

        var configuration = new Mock<IConfiguration>();
        configuration.SetupGet(x => x[It.Is<string>(s => s == "Buffering:Client:UseTemporaryFile")])
            .Returns("auto");
        configuration.SetupGet(x => x[It.Is<string>(s => s == "Buffering:Client:BufferSize")])
            .Returns("bad");
        configuration.SetupGet(x => x[It.Is<string>(s => s == "Buffering:Client:BigFileSize")])
            .Returns("0");

        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var controller = new UserFilesController(userFilesService.Object, configuration.Object, Mock.Of<ILogger<UserFilesController>>());
        controller.ControllerContext = controllerContext;

        // Act
        var response = await controller.DeleteFile(0, 0) as ObjectResult;

        // Assert
        Assert.Equal(StatusCodes.Status403Forbidden, response!.StatusCode);
    }

    [Fact]
    public async Task DownloadFile_Incorrect_User_Returns_NotFound()
    {
        // Arrange
        var userFilesService = new Mock<IUserFilesService>();
        userFilesService.Setup(x => x.DownloadFile(
            It.IsAny<CurrentUserDto?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<UserFileStreamDto> { StatusCode = ResultStatusCodes.Status404NotFound });

        var configuration = new Mock<IConfiguration>();
        configuration.SetupGet(x => x[It.Is<string>(s => s == "Buffering:Client:UseTemporaryFile")])
            .Returns("auto");
        configuration.SetupGet(x => x[It.Is<string>(s => s == "Buffering:Client:BufferSize")])
            .Returns("10");
        configuration.SetupGet(x => x[It.Is<string>(s => s == "Buffering:Client:BigFileSize")])
            .Returns("bad");

        var controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var controller = new UserFilesController(userFilesService.Object, configuration.Object, Mock.Of<ILogger<UserFilesController>>());
        controller.ControllerContext = controllerContext;

        // Act
        var response = await controller.DownloadFile(0, 0) as StatusCodeResult;

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, response!.StatusCode);
    }

}
