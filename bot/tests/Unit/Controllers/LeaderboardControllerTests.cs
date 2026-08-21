using LundBot.Config;
using LundBot.Controllers;
using LundBot.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace LundBot.Tests.Unit.Controllers;

public sealed class LeaderboardControllerTests
{
    [Fact]
    public async Task RefreshLeaderboard_WhenServiceSucceeds_ReturnsOk()
    {
        // Arrange
        var leaderboardService = new Mock<ILeaderboardService>();
        var controller = new LeaderboardController(
            Options.Create(new DeveloperEnvironmentConfig()),
            leaderboardService.Object
        );

        // Act
        IActionResult result = await controller.RefreshLeaderboard(10, 20);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        leaderboardService.Verify(s => s.RefreshLeaderboardAsync(10, 20), Times.Once);
    }

    [Fact]
    public async Task RefreshLeaderboard_WhenServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        var leaderboardService = new Mock<ILeaderboardService>();
        leaderboardService
            .Setup(s => s.RefreshLeaderboardAsync(10, 20))
            .ThrowsAsync(new Exception("boom"));
        var controller = new LeaderboardController(
            Options.Create(new DeveloperEnvironmentConfig()),
            leaderboardService.Object
        );

        // Act
        IActionResult result = await controller.RefreshLeaderboard(10, 20);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}
