using LundBot.Application.Discord.Channels;
using LundBot.Application.Discord.Interactions;
using LundBot.Application.Discord.Messages;
using LundBot.Domain.Common;

namespace LundBot.Application.Common.Messaging
{
    public interface IMessageService<TEntity, TFactory>
        where TEntity : AbstractMessageEntity, new()
        where TFactory : IMessageEntityFactory<TEntity>
    {
        TFactory MessageFactory { get; }

        Task SynchronizeDiscordMessagesAsync(string message, IEnumerable<TEntity> existingMessages, ulong channelId);

        Task DeleteMessagesForChannelAsync(IEnumerable<TEntity> existingMessages, DiscordChannelDto channel);

        Task DeleteMessageByIdAsync(TEntity message, DiscordChannelDto channel);

        Task CreateMessageWithComponentsAsync(
            string content,
            DiscordChannelDto channel,
            List<DiscordMessageComponentDto> components
        );

        Task CreateMessageFromDiscordMessageBuilderAsync(
            DiscordMessageBuilderDto messageBuilder,
            DiscordChannelDto channel,
            bool shouldSaveMessageInDatabase = false
        );
    }
}
