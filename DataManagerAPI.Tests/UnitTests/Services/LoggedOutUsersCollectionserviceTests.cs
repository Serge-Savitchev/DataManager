using DataManagerAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataManagerAPI.Tests.UnitTests.Services;

public class LoggedOutUsersCollectionserviceTests
{
    [Fact]
    public void Add_Remove_Contains_Return_Ok()
    {
        // Arrange
        var section = new Mock<IConfigurationSection>();
        section.SetupGet(x => x[It.Is<string>(s => s == "Redis")])
            .Returns("fake");

        var configuration = new Mock<IConfiguration>();
        configuration.SetupGet(x => x[It.Is<string>(s => s == "Tokens:AccessTokenLifetime")])
            .Returns("10");
        //configuration.SetupGet(x => x[It.Is<string>(s => s == "Redis")])
        //    .Returns((string)null!);
        configuration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(section.Object);

        var logger = Mock.Of<ILogger<LoggedOutUsersCollectionservice>>();

        var service = new LoggedOutUsersCollectionservice(configuration.Object, logger);

        // Act
        // Assert
        Assert.True(service.Add(1));
        Assert.True(service.Remove(1));
        Assert.False(service.Remove(1));
        Assert.False(service.Contains(1));

        Assert.True(service.Add(2));
        Assert.True(service.Contains(2));
        Assert.False(service.Add(2));
        Assert.True(service.Remove(2));
    }
}
