using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataManagerAPI.Tests.UnitTests.Services;

public class UserDataServiceTests
{
    [Fact]
    public async Task GetUserDataByUserId_Server_Error_Returns_InternalServerError()
    {
        // Arrange
        var repository = new Mock<IUserDataRepository>();
        repository.Setup(x => x.GetUserDataByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<UserData[]> { Success = false, StatusCode = ResultStatusCodes.Status500InternalServerError });

        var service = new UserDataService(repository.Object, null!, Mock.Of<ILogger<UserDataService>>());
        // Act
        var response = await service.GetUserDataByUserId(1);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, response.StatusCode);
    }
}
