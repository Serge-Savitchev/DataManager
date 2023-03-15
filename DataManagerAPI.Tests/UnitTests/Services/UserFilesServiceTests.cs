using AutoMapper;
using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DataManagerAPI.Tests.UnitTests.Services;

public class UserFilesServiceTests
{
    #region FeleteFile

    [Fact]
    public async Task DeleteFile_Null_CurrentUser_Returns_Unauthorized()
    {
        // Arrange
        var service = new UserFilesService(null!, null!, null!, Mock.Of<ILogger<UserFilesService>>());

        // Act
        var response = await service.DeleteFile(null, 1, 1);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFile_Server_Error_Returns_InternalServerError()
    {
        // Arrange
        //var dataRepository = new Mock<IUserDataRepository>();
        //dataRepository.Setup(x => x.GetUserAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(new ResultWrapper<User>
        //    {
        //        Success = false,
        //        StatusCode = ResultStatusCodes.Status500InternalServerError,
        //        Data = new User { Id = 1}
        //    });

        var repository = new Mock<IUserFilesRepository>();
        repository.Setup(x => x.DeleteFileAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<int> { Success = false, StatusCode = ResultStatusCodes.Status500InternalServerError });

        var service = new UserFilesService(repository.Object, null!, null!, Mock.Of<ILogger<UserFilesService>>());

        // Act
        var response = await service.DeleteFile(
            new CurrentUserDto
            {
                User = new UserDto { Id = 1, Role = RoleIds.Admin.ToString() }
            }, 1, 1);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }
    #endregion

    #region DownloadFile

    [Fact]
    public async Task DownloadFile_Null_CurrentUser_Returns_Unauthorized()
    {
        // Arrange
        var service = new UserFilesService(null!, null!, null!, Mock.Of<ILogger<UserFilesService>>());

        // Act
        var response = await service.DownloadFile(null, 1, 1);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DownloadFile_Server_Error_Returns_InternalServerError()
    {
        // Arrange
        var repository = new Mock<IUserFilesRepository>();
        repository.Setup(x => x.DownloadFileAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<UserFileStream> { Success = false, StatusCode = ResultStatusCodes.Status500InternalServerError });

        var service = new UserFilesService(repository.Object, null!, null!, Mock.Of<ILogger<UserFilesService>>());

        // Act
        var response = await service.DownloadFile(
            new CurrentUserDto
            {
                User = new UserDto { Id = 1, Role = RoleIds.Admin.ToString() }
            }, 1, 1);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }

    #endregion


    #region GetList

    [Fact]
    public async Task GetList_Null_CurrentUser_Returns_Unauthorized()
    {
        // Arrange
        var service = new UserFilesService(null!, null!, null!, Mock.Of<ILogger<UserFilesService>>());

        // Act
        var response = await service.GetList(null, 1);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetList_Server_Error_Returns_InternalServerError()
    {
        // Arrange
        var repository = new Mock<IUserFilesRepository>();
        repository.Setup(x => x.GetListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<UserFile[]> { Success = false, StatusCode = ResultStatusCodes.Status500InternalServerError });

        var service = new UserFilesService(repository.Object, null!, null!, Mock.Of<ILogger<UserFilesService>>());

        // Act
        var response = await service.GetList(
            new CurrentUserDto
            {
                User = new UserDto { Id = 1, Role = RoleIds.Admin.ToString() }
            }, 1);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }

    #endregion

    #region  UploadFile

    [Fact]
    public async Task UploadFile_Null_CurrentUser_Returns_Unauthorized()
    {
        // Arrange
        var service = new UserFilesService(null!, null!, null!, Mock.Of<ILogger<UserFilesService>>());

        // Act
        var response = await service.UploadFile(null, new UserFileStreamDto { Id = 1, Name = "Name", UserDataId = 1 });

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UploadFile_Server_Error_Returns_InternalServerError()
    {
        // Arrange
        var repository = new Mock<IUserFilesRepository>();
        repository.Setup(x => x.UploadFileAsync(It.IsAny<UserFileStream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<UserFile> { Success = false, StatusCode = ResultStatusCodes.Status500InternalServerError });

        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<UserFileStream>(It.IsAny<UserFileStreamDto>())).Returns(new UserFileStream());

        var service = new UserFilesService(repository.Object, null!, mapper.Object, Mock.Of<ILogger<UserFilesService>>());

        // Act
        var response = await service.UploadFile(
            new CurrentUserDto
            {
                User = new UserDto { Id = 1, Role = RoleIds.Admin.ToString() }
            }, new UserFileStreamDto());

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }

    #endregion
}
