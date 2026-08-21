using LundBot.Config;
using LundBot.Controllers;
using LundBot.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace LundBot.Tests.Unit.Controllers;

public sealed class CommandControllerTests
{
    [Fact]
    internal async Task SyncCommands_WhenCalled_ReturnsOk()
    {
        // Arrange
        var commandsService = new Mock<ICommandsService>();
        var controller = new CommandController(
            Options.Create(new DeveloperEnvironmentConfig()),
            commandsService.Object
        );

        // Act
        IActionResult result = await controller.SyncCommands();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        commandsService.Verify(s => s.RefreshCommands(), Times.Once);
    }

    [Fact]
    internal async Task UnregisterAllCommands_WhenServiceFails_ReturnsInternalServerError()
    {
        // Arrange
        var commandsService = new Mock<ICommandsService>();
        commandsService.Setup(s => s.UnregisterAllCommands(true)).ReturnsAsync(false);
        var controller = new CommandController(
            Options.Create(new DeveloperEnvironmentConfig()),
            commandsService.Object
        );

        // Act
        IActionResult result = await controller.UnregisterAllCommands(global: true);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}
