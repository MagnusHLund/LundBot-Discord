using System.Net;
using Xunit;

namespace LundBot.IntegrationTests.Api;

public sealed class ModerationControllerIntegrationTests : ApiIntegrationTestBase
{
    [Fact]
    public async Task Assign_requires_authentication()
    {
        using HttpResponseMessage response = await SendJsonAsync(
            HttpMethod.Post,
            "/api/moderation/kick/roles/assign",
            new
            {
                guildId = ApiTestIds.GuildId,
                roleId = ApiTestIds.RoleId,
                kickReason = "Test reason",
            }
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Assign_persists_the_role_rule()
    {
        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Post,
            "/api/moderation/kick/roles/assign",
            new
            {
                guildId = ApiTestIds.GuildId,
                roleId = ApiTestIds.RoleId,
                kickReason = "Test reason",
            }
        );

        int matchingRules = await ApiTestData.CountAutoKickRolesAsync(Host, ApiTestIds.GuildId, ApiTestIds.RoleId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(1, matchingRules);
    }

    [Fact]
    public async Task Assign_reports_duplicate_role_rule()
    {
        await ApiTestData.SeedAutoKickRoleAsync(Host, ApiTestIds.GuildId, ApiTestIds.RoleId);

        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Post,
            "/api/moderation/kick/roles/assign",
            new
            {
                guildId = ApiTestIds.GuildId,
                roleId = ApiTestIds.RoleId,
                kickReason = "Duplicate",
            }
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Assign_rejects_invalid_guild_id()
    {
        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Post,
            "/api/moderation/kick/roles/assign",
            new
            {
                guildId = 1,
                roleId = ApiTestIds.RoleId,
                kickReason = "Invalid guild",
            }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Unassign_requires_authentication()
    {
        using HttpResponseMessage response = await SendJsonAsync(
            HttpMethod.Delete,
            "/api/moderation/kick/roles/unassign",
            new { guildId = ApiTestIds.GuildId, roleId = ApiTestIds.RoleId }
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Unassign_removes_existing_role_rule()
    {
        await ApiTestData.SeedAutoKickRoleAsync(Host, ApiTestIds.GuildId, ApiTestIds.RoleId);

        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Delete,
            "/api/moderation/kick/roles/unassign",
            new { guildId = ApiTestIds.GuildId, roleId = ApiTestIds.RoleId }
        );

        int remainingRules = await ApiTestData.CountAutoKickRolesAsync(Host, ApiTestIds.GuildId, ApiTestIds.RoleId);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(0, remainingRules);
    }

    [Fact]
    public async Task Unassign_reports_missing_role_rule()
    {
        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Delete,
            "/api/moderation/kick/roles/unassign",
            new { guildId = ApiTestIds.GuildId, roleId = ApiTestIds.RoleId }
        );

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Unassign_rejects_invalid_role_id()
    {
        using HttpResponseMessage response = await SendAuthorizedJsonAsync(
            HttpMethod.Delete,
            "/api/moderation/kick/roles/unassign",
            new { guildId = ApiTestIds.GuildId, roleId = 1 }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
