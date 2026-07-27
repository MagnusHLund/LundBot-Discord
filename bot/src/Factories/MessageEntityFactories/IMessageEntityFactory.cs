using LundBot.Entities;

namespace LundBot.Factories.MessageEntityFactories
{
    public interface IMessageEntityFactory<TEntity>
        where TEntity : AbstractMessageEntity, new()
    {
        TEntity Create(string discordMessageId);
    }
}
