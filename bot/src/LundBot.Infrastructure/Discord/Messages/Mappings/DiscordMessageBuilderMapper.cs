using DSharpPlus.Entities;
using LundBot.Application.Discord.Messages;

namespace LundBot.Infrastructure.Discord.Messages.Mappings
{
    public sealed class DiscordMessageBuilderMapper
    {
        public static DiscordMessageBuilder Map(DiscordMessageBuilderDto messageBuilder)
        {
            var builder = new DiscordMessageBuilder().WithContent(messageBuilder.Content);

            if (messageBuilder.ReplyToMessageId.HasValue)
            {
                builder.WithReply(messageBuilder.ReplyToMessageId.Value);
            }

            return builder;
        }
    }
}
