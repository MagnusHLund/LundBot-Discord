using System.Net;

namespace LundBot.Tests.Integration.Api;

public sealed class ApiHttpRequestsIntegrationTests
{
    [Fact]
    public async Task GetHealthEndpoint_WhenCalled_ReturnsHealthyPayload()
    {
        // Arrange
        (HttpClient client, _, _, _) = await ApiTestServerFactory.CreateAsync();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/health");
        string body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Healthy", body);
        Assert.Contains("9.9.9", body);
    }

    [Fact]
    public async Task PostCommandSyncEndpoint_WhenCalled_InvokesCommandsService()
    {
        // Arrange
        (HttpClient client, SpyCommandsService commands, _, _) = await ApiTestServerFactory.CreateAsync();

        // Act
        HttpResponseMessage response = await client.PostAsync("/api/command/sync", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(commands.RefreshCommandsCalled);
    }

    [Fact]
    public async Task DeleteUnregisterAllEndpoint_WhenServiceFails_ReturnsInternalServerError()
    {
        // Arrange
        (HttpClient client, SpyCommandsService commands, _, _) = await ApiTestServerFactory.CreateAsync();
        commands.UnregisterAllShouldSucceed = false;

        // Act
        HttpResponseMessage response = await client.DeleteAsync(
            "/api/command/unregister/all?global=true"
        );

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task PostTrafficVisitEndpoint_WhenServiceSucceeds_ReturnsOk()
    {
        // Arrange
        (HttpClient client, _, _, SpyWebsiteTrafficService traffic) =
            await ApiTestServerFactory.CreateAsync();
        traffic.RegisterVisitResult = true;

        // Act
        HttpResponseMessage response = await client.PostAsync("/api/traffic/visit", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostLeaderboardRefreshEndpoint_WhenServiceThrows_ReturnsInternalServerError()
    {
        // Arrange
        (HttpClient client, _, SpyLeaderboardService leaderboardService, _) =
            await ApiTestServerFactory.CreateAsync();
        leaderboardService.ThrowOnRefresh = true;

        // Act
        HttpResponseMessage response = await client.PostAsync(
            "/api/leaderboard/refresh?channelId=10&guildId=11",
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
