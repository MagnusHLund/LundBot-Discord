using System.Net;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Api;

public sealed class CommandControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task Sync_requires_authentication()
    {
        using HttpResponseMessage response = await Host.Client.PostAsync("/api/command/sync", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Sync_returns_no_content_when_service_succeeds()
    {
        Host.CommandService.RefreshCommandsAsync().Returns(true);

        using HttpResponseMessage response = await SendAuthorizedAsync(HttpMethod.Post, "/api/command/sync");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Sync_returns_server_error_when_service_fails()
    {
        Host.CommandService.RefreshCommandsAsync().Returns(false);

        using HttpResponseMessage response = await SendAuthorizedAsync(HttpMethod.Post, "/api/command/sync");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterAll_requires_authentication()
    {
        using HttpResponseMessage response = await Host.Client.DeleteAsync("/api/command/unregister/all");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterAll_without_guild_succeeds()
    {
        Host.CommandService.UnregisterAllCommands(null).Returns(true);

        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            "/api/command/unregister/all"
        );

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterAll_for_guild_succeeds()
    {
        Host.CommandService.UnregisterAllCommands(ApiTestIds.GuildId).Returns(true);

        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"/api/command/unregister/all?guildId={ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterAll_returns_server_error_when_service_fails()
    {
        Host.CommandService.UnregisterAllCommands(ApiTestIds.GuildId).Returns(false);

        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"/api/command/unregister/all?guildId={ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterAll_rejects_invalid_guild_id()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            "/api/command/unregister/all?guildId=1"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterOne_requires_authentication()
    {
        using HttpResponseMessage response = await Host.Client.DeleteAsync(
            $"/api/command/unregister/{ApiTestIds.CommandId}"
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterOne_succeeds_for_valid_ids()
    {
        Host.CommandService.UnregisterCommand(ApiTestIds.CommandId, ApiTestIds.GuildId).Returns(true);

        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"/api/command/unregister/{ApiTestIds.CommandId}?guildId={ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterOne_returns_server_error_when_service_fails()
    {
        Host.CommandService.UnregisterCommand(ApiTestIds.CommandId, ApiTestIds.GuildId).Returns(false);

        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"/api/command/unregister/{ApiTestIds.CommandId}?guildId={ApiTestIds.GuildId}"
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterOne_rejects_invalid_command_id()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(HttpMethod.Delete, "/api/command/unregister/1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterOne_rejects_invalid_guild_id()
    {
        using HttpResponseMessage response = await SendAuthorizedAsync(
            HttpMethod.Delete,
            $"/api/command/unregister/{ApiTestIds.CommandId}?guildId=1"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
