﻿using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataManagerAPI.Tests.UnitTests.Services;

public class UsersServiceTests
{
    [Fact]
    public async Task GetUsersByRole_Server_Error_Retutns_InternalServerError()
    {
        // Arrange
        var repository = new Mock<IUsersRepository>();
        repository.Setup(x => x.GetUsersByRoleAsync(It.IsAny<RoleIds>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<User[]> { Success = false, StatusCode = ResultStatusCodes.Status500InternalServerError });

        var service = new UsersService(repository.Object, null!, Mock.Of<ILogger<UsersService>>());
        // Act
        var response = await service.GetUsersByRole("user");

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }
}
