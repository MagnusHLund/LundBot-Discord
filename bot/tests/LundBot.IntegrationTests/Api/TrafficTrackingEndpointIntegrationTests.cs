using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace LundBot.IntegrationTests.Api;

public sealed class TrafficTrackingEndpointIntegrationTests : ApiIntegrationTestBase
{
    private const string VisitorIp = "198.51.100.20";
    private const string OtherVisitorIp = "198.51.100.21";

    [Fact]
    public async Task Visit_registers_visitor()
    {
        int trafficChannelId = await ApiTestData.SeedTrafficChannelAsync(
            Host,
            ApiTestIds.GuildId,
            ApiTestIds.ChannelId
        );
        using HttpRequestMessage request = CreateTrafficRequest("/api/traffic/visit", VisitorIp);

        using HttpResponseMessage response = await Host.Client.SendAsync(request);

        int visits = await ApiTestData.CountWebsiteVisitsAsync(Host, trafficChannelId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(1, visits);
    }

    [Fact]
    public async Task Visit_fails_without_registered_analytics_channel()
    {
        using HttpResponseMessage response = await Host.Client.PostAsJsonAsync(
            "/api/traffic/visit",
            new { guildId = ApiTestIds.GuildId }
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Visit_rejects_duplicate_visitor()
    {
        int trafficChannelId = await ApiTestData.SeedTrafficChannelAsync(
            Host,
            ApiTestIds.GuildId,
            ApiTestIds.ChannelId
        );
        await ApiTestData.SeedWebsiteVisitAsync(Host, trafficChannelId, VisitorIp);
        using HttpRequestMessage request = CreateTrafficRequest("/api/traffic/visit", VisitorIp);

        using HttpResponseMessage response = await Host.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Visit_rejects_invalid_guild_id()
    {
        using HttpResponseMessage response = await Host.Client.PostAsJsonAsync(
            "/api/traffic/visit",
            new { guildId = 1 }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task InviteClick_marks_existing_visit()
    {
        int trafficChannelId = await ApiTestData.SeedTrafficChannelAsync(
            Host,
            ApiTestIds.GuildId,
            ApiTestIds.ChannelId
        );
        await ApiTestData.SeedWebsiteVisitAsync(Host, trafficChannelId, VisitorIp);
        using HttpRequestMessage request = CreateTrafficRequest("/api/traffic/invite-click", VisitorIp);

        using HttpResponseMessage response = await Host.Client.SendAsync(request);

        bool wasMarkedClicked = await ApiTestData.WasInviteClickedAsync(Host, trafficChannelId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(wasMarkedClicked);
    }

    [Fact]
    public async Task InviteClick_fails_when_visitor_has_not_visited()
    {
        await ApiTestData.SeedTrafficChannelAsync(Host, ApiTestIds.GuildId, ApiTestIds.ChannelId);
        using HttpRequestMessage request = CreateTrafficRequest("/api/traffic/invite-click", OtherVisitorIp);

        using HttpResponseMessage response = await Host.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task InviteClick_fails_without_registered_analytics_channel()
    {
        using HttpResponseMessage response = await Host.Client.PostAsJsonAsync(
            "/api/traffic/invite-click",
            new { guildId = ApiTestIds.GuildId }
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task InviteClick_rejects_invalid_guild_id()
    {
        using HttpResponseMessage response = await Host.Client.PostAsJsonAsync(
            "/api/traffic/invite-click",
            new { guildId = 1 }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static HttpRequestMessage CreateTrafficRequest(string path, string ipAddress)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = JsonContent.Create(new { guildId = ApiTestIds.GuildId }),
        };
        request.Headers.Add("X-Forwarded-For", ipAddress);
        return request;
    }
}
