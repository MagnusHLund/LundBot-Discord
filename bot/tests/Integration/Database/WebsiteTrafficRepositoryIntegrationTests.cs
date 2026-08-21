using LundBot.Entities;
using LundBot.Repositories;
using LundBot.Tests.Fixtures.Data;
using LundBot.Utils;

namespace LundBot.Tests.Integration.Database;

public sealed class WebsiteTrafficRepositoryIntegrationTests
{
    [Fact]
    internal async Task RegisterWebsiteVisitAndInviteClickAsync_WhenVisitExists_UpdatesInviteFlag()
    {
        // Arrange
        using var fixture = new SqliteDbFixture();
        var repository = new WebsiteTrafficRepository(fixture.Db);
        byte[] hashedIp = HashUtils.HashString("127.0.0.1");

        // Act
        bool visitSaved = await repository.RegisterWebsiteVisitAsync(hashedIp);
        bool clickSaved = await repository.RegisterInviteLinkClickAsync(hashedIp);
        List<WebsiteTrafficEntity> entities =
            await repository.GetWebsiteTrafficEntitiesForPeriodAsync(
                DateTime.UtcNow.AddMinutes(-5),
                DateTime.UtcNow.AddMinutes(5)
            );

        // Assert
        Assert.True(visitSaved);
        Assert.True(clickSaved);
        WebsiteTrafficEntity entity = Assert.Single(entities);
        Assert.True(entity.ClickedInviteButton);
    }
}
