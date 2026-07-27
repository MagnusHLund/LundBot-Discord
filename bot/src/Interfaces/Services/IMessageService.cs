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
    }
}
