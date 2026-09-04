namespace LundBot.Application.Common.Discord
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
