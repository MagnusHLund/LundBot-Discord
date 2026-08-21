using LundBot.Config;
using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Repositories;
using LundBot.Services;
using LundBot.Tests.Fixtures.Data;
using Microsoft.Extensions.Options;
using Moq;

namespace LundBot.Tests.Unit.Services;

public sealed class WebsiteTrafficServiceTests
{
    [Fact]
    internal async Task RegisterWebsiteVisitAsync_WhenRepositorySucceeds_UpdatesStatsMessage()
    {
        // Arrange
        using var fixture = new SqliteDbFixture();
        var websiteTrafficRepository = new Mock<IWebsiteTrafficRepository>();
        websiteTrafficRepository
            .Setup(r => r.RegisterWebsiteVisitAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(true);
        websiteTrafficRepository
            .Setup(r =>
                r.GetWebsiteTrafficEntitiesForPeriodAsync(
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()
                )
            )
            .ReturnsAsync(new List<WebsiteTrafficEntity>());

        var trafficMessagesRepository = new WebsiteTrafficMessagesRepository(fixture.Db);

        var messageService =
            new Mock<
                IMessageService<
                    WebsiteTrafficMessagesEntity,
                    WebsiteTrafficMessagesRepository,
                    WebsiteTrafficMessageFactory
                >
            >();

        var service = new WebsiteTrafficService(
            Options.Create(new DiscordConfig { WebTrafficChannelId = 42 }),
            messageService.Object,
            websiteTrafficRepository.Object,
            trafficMessagesRepository
        );

        // Act
        bool result = await service.RegisterWebsiteVisitAsync("10.0.0.1");

        // Assert
        Assert.True(result);
        messageService.Verify(
            m =>
                m.SynchronizeDiscordMessagesAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<WebsiteTrafficMessagesEntity>>(),
                    42
                ),
            Times.Once
        );
    }
}
