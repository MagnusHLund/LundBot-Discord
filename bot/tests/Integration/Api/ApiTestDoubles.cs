using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Interfaces.Services;

namespace LundBot.Tests.Integration.Api;

internal sealed class SpyCommandsService : ICommandsService
{
    public bool RefreshCommandsCalled { get; private set; }
    public bool UnregisterAllShouldSucceed { get; set; } = true;

    public Task RegisterCommandsAsync() => Task.CompletedTask;
    public Task LogRegisteredCommandsForGuildsAsync() => Task.CompletedTask;

    public Task RefreshCommands()
    {
        RefreshCommandsCalled = true;
        return Task.CompletedTask;
    }

    public Task<bool> UnregisterCommand(string commandId, bool global = false) =>
        Task.FromResult(true);

    public Task<bool> UnregisterAllCommands(bool global = false) =>
        Task.FromResult(UnregisterAllShouldSucceed);
}

internal sealed class SpyLeaderboardService : ILeaderboardService
{
    public bool ThrowOnRefresh { get; set; }
    public bool RefreshCalled { get; private set; }

    public Task CreateLeaderboardAsync(
        DiscordChannel channel,
        string title,
        string message,
        LundBot.Enums.LeaderboardType leaderboardType
    ) => Task.CompletedTask;

    public Task RemoveLeaderboardAsync(DiscordChannel channel) => Task.CompletedTask;

    public Task UpvoteUserOnLeaderboardAsync(
        DiscordChannel channel,
        DiscordUser userUpvoting,
        DiscordUser userTarget
    ) => Task.CompletedTask;

    public Task RegisterUserJoinedWithInviteAsync(
        DiscordGuild guild,
        DiscordUser userJoined,
        DiscordUser userInvitedBy
    ) => Task.CompletedTask;

    public Task RegisterWarningOnLeaderboardAsync(DiscordChannel channel, DiscordUser userTarget) =>
        Task.CompletedTask;

    public Task RefreshLeaderboardAsync(ulong channelId, ulong guildId)
    {
        RefreshCalled = true;
        if (ThrowOnRefresh)
        {
            throw new Exception("Refresh failed");
        }

        return Task.CompletedTask;
    }

    public ValueTask<List<LeaderboardsEntity>> GetLeaderboardsForGuildAsync(string guildId) =>
        ValueTask.FromResult(new List<LeaderboardsEntity>());

    public Task UpdateLeaderboardMessageAsync(LeaderboardsEntity leaderboard, DiscordChannel channel) =>
        Task.CompletedTask;
}

internal sealed class SpyWebsiteTrafficService : IWebsiteTrafficService
{
    public bool RegisterVisitResult { get; set; } = true;
    public bool RegisterInviteClickResult { get; set; } = true;

    public Task<bool> RegisterWebsiteVisitAsync(string ipAddress) =>
        Task.FromResult(RegisterVisitResult);

    public Task<bool> RegisterInviteLinkClickAsync(string ipAddress) =>
        Task.FromResult(RegisterInviteClickResult);
}
