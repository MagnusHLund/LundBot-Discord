using LundBot.Application.Common.Security.Hashing;
using LundBot.Domain.Moderation;
using LundBot.Domain.WebsiteTraffic;

namespace LundBot.IntegrationTests.Api;

internal static class ApiTestData
{
    public static Task SeedAutoKickRoleAsync(ApiTestHost host, ulong guildId, ulong roleId) =>
        host.WithDbContextAsync(async db =>
        {
            db.AutoKickRoles.Add(new AutoKickRole(roleId, guildId, "Seeded test rule"));
            await db.SaveChangesAsync();
            return true;
        });

    public static Task<int> CountAutoKickRolesAsync(ApiTestHost host, ulong guildId, ulong roleId) =>
        host.WithDbContextAsync(db =>
            Task.FromResult(db.AutoKickRoles.Count(role => role.GuildId == guildId && role.RoleId == roleId))
        );

    public static Task<int> SeedTrafficChannelAsync(ApiTestHost host, ulong guildId, ulong channelId) =>
        host.WithDbContextAsync(async db =>
        {
            WebsiteTrafficAnalyticsChannel channel = new WebsiteTrafficAnalyticsChannel
            {
                GuildId = guildId,
                ChannelId = channelId,
            };
            db.WebsiteTrafficAnalyticsChannels.Add(channel);
            await db.SaveChangesAsync();
            return channel.Id;
        });

    public static Task<int> CountTrafficChannelsAsync(ApiTestHost host, ulong guildId) =>
        host.WithDbContextAsync(db =>
            Task.FromResult(db.WebsiteTrafficAnalyticsChannels.Count(channel => channel.GuildId == guildId))
        );

    public static Task SeedWebsiteVisitAsync(ApiTestHost host, int trafficChannelId, string ipAddress) =>
        host.WithDbContextAsync(async db =>
        {
            db.WebsiteTraffic.Add(
                new WebsiteTrafficAnalytics
                {
                    HashedIp = HashUtils.HashString(ipAddress),
                    WebsiteTrafficAnalyticsChannelId = trafficChannelId,
                }
            );
            await db.SaveChangesAsync();
            return true;
        });

    public static Task<int> CountWebsiteVisitsAsync(ApiTestHost host, int trafficChannelId) =>
        host.WithDbContextAsync(db =>
            Task.FromResult(
                db.WebsiteTraffic.Count(visit => visit.WebsiteTrafficAnalyticsChannelId == trafficChannelId)
            )
        );

    public static Task<bool> WasInviteClickedAsync(ApiTestHost host, int trafficChannelId) =>
        host.WithDbContextAsync(db =>
            Task.FromResult(
                db.WebsiteTraffic.Any(visit =>
                    visit.WebsiteTrafficAnalyticsChannelId == trafficChannelId && visit.ClickedInviteButton
                )
            )
        );
}
