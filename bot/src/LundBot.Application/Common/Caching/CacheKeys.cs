namespace LundBot.Application.Common.Caching
{
    public static class CacheKeys
    {
        public static string LeaderboardsPerGuild(ulong guildId) => $"guild_leaderboards_{guildId}";

        public static string GuildInvites(ulong guildId) => $"guild_invites_{guildId}";
    }
}
