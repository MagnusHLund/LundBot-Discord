using DSharpPlus.Entities;
using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Repositories;

namespace LundBot.Interfaces.Services
{
    public interface IMessageService<TEntity, TRepository, TFactory>
        where TRepository : AbstractMessageRepository<TEntity>
        where TEntity : AbstractMessageEntity, new()
        where TFactory : IMessageEntityFactory<TEntity>
    {
        TFactory MessageFactory { get; }

        Task SynchronizeDiscordMessagesAsync(
            string message,
            IEnumerable<TEntity> existingMessages,
            ulong channelId
        );

        Task DeleteMessagesForChannelAsync(
            IEnumerable<TEntity> existingMessages,
            DiscordChannel channel
        );

        Task DeleteMessageByIdAsync(TEntity message, DiscordChannel channel);

        Task CreateMessageWithComponentsAsync(
            string content,
            DiscordChannel channel,
            List<DiscordComponent> components
        );

        Task CreateMessageFromDiscordMessageBuilderAsync(
            DiscordMessageBuilder messageBuilder,
            DiscordChannel channel,
            bool shouldSaveMessageInDatabase = false
        );
    }
}
