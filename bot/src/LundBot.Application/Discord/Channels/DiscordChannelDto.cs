namespace LundBot.Application.Discord.Channels
{
    public sealed record DiscordChannelDto
    {
        public ulong ChannelId { get; }

        public DiscordChannelDto(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}
