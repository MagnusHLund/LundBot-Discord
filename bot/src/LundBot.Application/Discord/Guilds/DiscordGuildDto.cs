namespace LundBot.Application.Discord.Guilds
{
    public sealed record DiscordGuildDto
    {
        public ulong GuildId { get; }
        public string GuildName { get; }

        public DiscordGuildDto(ulong guildId, string guildName)
        {
            GuildId = guildId;
            GuildName = guildName;
        }
    }
}
