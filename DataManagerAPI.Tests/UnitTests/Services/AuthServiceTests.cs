using DataManagerAPI.Dto;
using DataManagerAPI.Repository.Abstractions.Constants;
using DataManagerAPI.Repository.Abstractions.Helpers;
using DataManagerAPI.Repository.Abstractions.Interfaces;
using DataManagerAPI.Repository.Abstractions.Models;
using DataManagerAPI.Services;
using DataManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace DataManagerAPI.Tests.UnitTests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RefreshToken_Empty_Claims_Returns_Unauthorized()
    {
        // Arrange
        var tokenSevice = new Mock<ITokenService>();
        tokenSevice.Setup(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(new ClaimsPrincipal());
        tokenSevice.Setup(x => x.CreateCurrentUser(It.IsAny<IEnumerable<Claim>>()))
            .Returns((CurrentUserDto?)null!);

        var service = new AuthService(null!, null!, tokenSevice.Object, null!, null!, Mock.Of<ILogger<AuthService>>());
        // Act
        var response = await service.RefreshToken(new TokenApiModelDto { AccessToken = "" });

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_Incorrect_User_Returns_NotFound()
    {
        // Arrange
        var repository = new Mock<IAuthRepository>();
        repository.Setup(x => x.GetUserDetailsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultWrapper<UserCredentialsData> { Success = false, StatusCode = ResultStatusCodes.Status404NotFound });

        var tokenSevice = new Mock<ITokenService>();
        tokenSevice.Setup(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(new ClaimsPrincipal());
        tokenSevice.Setup(x => x.CreateCurrentUser(It.IsAny<IEnumerable<Claim>>()))
            .Returns(new CurrentUserDto { User = new UserDto() });

        var service = new AuthService(repository.Object, null!, tokenSevice.Object, null!, null!, Mock.Of<ILogger<AuthService>>());
        // Act
        var response = await service.RefreshToken(new TokenApiModelDto { AccessToken = "" });

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
    }
}
