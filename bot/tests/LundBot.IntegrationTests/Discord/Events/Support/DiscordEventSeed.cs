using LundBot.Application.Common.Caching;
using LundBot.Application.Discord.Invites;
using LundBot.Application.Discord.Users;
using LundBot.Domain.Leaderboards;
using LundBot.Domain.MemberJoin;
using LundBot.Domain.Moderation;
using NSubstitute;

namespace LundBot.IntegrationTests.Discord.Events.Support;

internal static class DiscordEventSeed
{
    public static async Task WelcomeAsync(DiscordEventTestHost host)
    {
        host.Db.MemberJoinMessages.Add(
            new MemberJoinMessage
            {
                DiscordUserId = DiscordEventData.TargetId,
                DiscordMessageId = DiscordEventData.MessageId,
            }
        );
        await host.Db.SaveChangesAsync();
    }

    public static async Task KickRoleAsync(DiscordEventTestHost host)
    {
        host.Db.AutoKickRoles.Add(
            new AutoKickRole(DiscordEventData.RoleId, DiscordEventData.GuildId, "Onboarding rule")
        );
        await host.Db.SaveChangesAsync();
    }

    public static async Task InviteLeaderboardAsync(DiscordEventTestHost host)
    {
        host.Db.Leaderboards.Add(
            new Leaderboard
            {
                DiscordServerId = DiscordEventData.GuildId,
                DiscordChannelId = DiscordEventData.ChannelId,
                Title = "Invites",
                Message = "Invite members",
                LeaderboardType = LeaderboardTypeEnum.Invite,
            }
        );
        await host.Db.SaveChangesAsync();
    }

    public static void InviteUsed(DiscordEventTestHost host, DiscordUserDto? inviter)
    {
        host.Cache.Set(
            CacheKeys.GuildInvites(DiscordEventData.GuildId),
            new List<DiscordInviteDto> { new("test-invite", 0, inviter) }
        );
        host.Guilds.GetGuildInvitesAsync(DiscordEventData.GuildId)
            .Returns(new List<DiscordInviteDto> { new("test-invite", 1, inviter) });
    }
}
