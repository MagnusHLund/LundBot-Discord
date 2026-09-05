using DSharpPlus.Entities;
using LundBot.Application.Discord.Channels;

namespace LundBot.Infrastructure.Discord.Channels.Mappings
{
    public static class DiscordChannelMapper
    {
        public static DiscordChannelDto Map(DiscordChannel channel)
        {
            return new DiscordChannelDto(channel.Id);
        }
    }
}
