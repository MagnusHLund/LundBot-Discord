namespace LundBot.Helpers
{
    public class CacheKeyHelper
    {
        public static string LeaderboardsPerGuild(string guildId) =>
            $"guild_leaderboards_{guildId}";

        public static string GuildInvites(string guild) => $"guild_invites_{guild}";
    }
}
