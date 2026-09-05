using DSharpPlus.Entities;
using LundBot.Application.Discord.Messages;

namespace LundBot.Infrastructure.Discord.Messages.Mappings
{
    public static class DiscordMessageMapper
    {
        public static DiscordMessageDto Map(DiscordMessage message)
        {
            return new DiscordMessageDto(
                messageId: message.Id,
                channelId: message?.Channel?.Id ?? 0,
                authorId: message?.Author?.Id ?? 0,
                content: message?.Content ?? string.Empty
            );
        }
    }
}
