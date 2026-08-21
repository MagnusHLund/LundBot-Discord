using LundBot.Config;
using LundBot.Controllers;
using LundBot.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace LundBot.Tests.Unit.Controllers;

public sealed class TrafficControllerTests
{
    [Fact]
    public async Task VisitedWebsite_WhenServiceSucceeds_ReturnsOk()
    {
        // Arrange
        var trafficService = new Mock<IWebsiteTrafficService>();
        trafficService.Setup(s => s.RegisterWebsiteVisitAsync(It.IsAny<string>())).ReturnsAsync(true);

        var controller = new TrafficController(
            trafficService.Object,
            Options.Create(new DeveloperEnvironmentConfig())
        )
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };

        // Act
        IActionResult result = await controller.VisitedWebsite();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        trafficService.Verify(s => s.RegisterWebsiteVisitAsync(It.IsAny<string>()), Times.Once);
    }
}
