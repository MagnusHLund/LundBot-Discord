using System.Net;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Api;

public sealed class LeaderboardControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task Refresh_requires_authentication()
    {
        using HttpResponseMessage response = await Host.Client.PostAsync(
            $"/api/leaderboard/refresh?channelId={ApiTestIds.ChannelId}&guildId={ApiTestIds.GuildId}",
            null
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_succeeds_for_valid_ids()
    {
        Host.LeaderboardService.RefreshLeaderboardAsync(ApiTestIds.ChannelId, ApiTestIds.GuildId).Returns(true);

        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Post,
            $"/api/leaderboard/refresh?channelId={ApiTestIds.ChannelId}&guildId={ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_returns_server_error_when_service_fails()
    {
        Host.LeaderboardService.RefreshLeaderboardAsync(ApiTestIds.ChannelId, ApiTestIds.GuildId).Returns(false);

        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Post,
            $"/api/leaderboard/refresh?channelId={ApiTestIds.ChannelId}&guildId={ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_rejects_invalid_channel_id()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Post,
            $"/api/leaderboard/refresh?channelId=1&guildId={ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_requires_guild_id()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Post,
            $"/api/leaderboard/refresh?channelId={ApiTestIds.ChannelId}"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_requires_channel_id()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Post,
            $"/api/leaderboard/refresh?guildId={ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("channelId=1531717575338102914")]
    [InlineData("guildId=1531716596022644876")]
    [InlineData("channelId=1&guildId=1531716596022644876")]
    [InlineData("channelId=1531717575338102914&guildId=1")]
    public async Task Refresh_does_not_call_service_for_invalid_query(string query)
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Post,
            $"/api/leaderboard/refresh?{query}"
        );

        await Host.LeaderboardService.DidNotReceive()
            .RefreshLeaderboardAsync(Arg.Any<ulong>(), Arg.Any<ulong>());
    }
}
