namespace LundBot.Application.Discord.Channels
{
    public sealed record DiscordChannelDto
    {
        public ulong ChannelId { get; }
        public ulong GuildId { get; }

        public DiscordChannelDto(ulong channelId, ulong guildId)
        {
            ChannelId = channelId;
            GuildId = guildId;
        }
    }
}
