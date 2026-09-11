using LundBot.Domain.Common;

namespace LundBot.Application.Common.Messaging
{
    public interface IMessageEntityFactory<TEntity>
        where TEntity : AbstractMessageEntity, new()
    {
        TEntity Create(ulong discordMessageId);
    }
}
