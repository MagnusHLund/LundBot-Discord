namespace LundBot.Application.Discord.Guilds
{
    public sealed record DiscordGuildDto
    {
        public ulong GuildId { get; }

        public DiscordGuildDto(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
