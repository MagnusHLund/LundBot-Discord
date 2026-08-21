using LundBot.Config;
using LundBot.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LundBot.Tests.Unit.Controllers;

public sealed class HealthControllerTests
{
    [Fact]
    internal void Get_WhenCalled_ReturnsHealthyStatusAndVersion()
    {
        // Arrange
        var controller = new HealthController(
            Options.Create(new DeveloperEnvironmentConfig()),
            Options.Create(new ServerConfig { Version = "1.2.3" })
        );

        // Act
        IActionResult result = controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        string payload = okResult.Value?.ToString() ?? string.Empty;
        Assert.Contains("Healthy", payload);
        Assert.Contains("1.2.3", payload);
    }
}
