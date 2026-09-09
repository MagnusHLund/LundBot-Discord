using LundBot.Domain.Common;

namespace LundBot.Application.Common.Messaging
{
    public interface IMessageRepository<TEntity>
        where TEntity : AbstractMessageEntity, new()
    {
        Task<bool> CreateAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteManyAsync(IEnumerable<int> ids);
    }
}
