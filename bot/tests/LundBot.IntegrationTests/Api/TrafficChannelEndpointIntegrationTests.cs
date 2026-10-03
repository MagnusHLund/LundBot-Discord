using System.Net;
using Xunit;

namespace LundBot.IntegrationTests.Api;

public sealed class TrafficChannelEndpointIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task CreateChannel_requires_authentication()
    {
        using HttpResponseMessage response = await SendJsonAsync(
            HttpMethod.Post,
            "/api/traffic/create-channel",
            new { guildId = ApiTestIds.GuildId, channelId = ApiTestIds.ChannelId }
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateChannel_persists_analytics_channel()
    {
        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Post,
            "/api/traffic/create-channel",
            new { guildId = ApiTestIds.GuildId, channelId = ApiTestIds.ChannelId }
        );

        int channelCount = await ApiTestData.CountTrafficChannelsAsync(Host, ApiTestIds.GuildId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(1, channelCount);
    }

    [Fact]
    public async Task CreateChannel_reports_duplicate_guild()
    {
        await ApiTestData.SeedTrafficChannelAsync(Host, ApiTestIds.GuildId, ApiTestIds.ChannelId);

        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Post,
            "/api/traffic/create-channel",
            new { guildId = ApiTestIds.GuildId, channelId = ApiTestIds.ChannelId + 1 }
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task CreateChannel_rejects_invalid_channel_id()
    {
        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Post,
            "/api/traffic/create-channel",
            new { guildId = ApiTestIds.GuildId, channelId = 1 }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RemoveChannel_requires_authentication()
    {
        using HttpResponseMessage response = await Host.Client.DeleteAsync(
            $"/api/traffic/remove-channel/{ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RemoveChannel_deletes_existing_channel()
    {
        await ApiTestData.SeedTrafficChannelAsync(Host, ApiTestIds.GuildId, ApiTestIds.ChannelId);

        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"/api/traffic/remove-channel/{ApiTestIds.GuildId}"
        );

        int remainingChannels = await ApiTestData.CountTrafficChannelsAsync(Host, ApiTestIds.GuildId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(0, remainingChannels);
    }

    [Fact]
    public async Task RemoveChannel_reports_missing_channel()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"/api/traffic/remove-channel/{ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task RemoveChannel_rejects_invalid_guild_id()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            "/api/traffic/remove-channel/1"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
