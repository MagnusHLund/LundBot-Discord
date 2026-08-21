using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Enums;
using LundBot.Interfaces.Services;

namespace LundBot.Tests.Integration.Api;

internal sealed class SpyLeaderboardService : ILeaderboardService
{
    internal bool ThrowOnRefresh { get; set; }
    internal bool RefreshCalled { get; private set; }

    public Task CreateLeaderboardAsync(
        DiscordChannel channel,
        string title,
        string message,
        LeaderboardType leaderboardType
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

    public Task UpdateLeaderboardMessageAsync(
        LeaderboardsEntity leaderboard,
        DiscordChannel channel
    ) => Task.CompletedTask;
}
