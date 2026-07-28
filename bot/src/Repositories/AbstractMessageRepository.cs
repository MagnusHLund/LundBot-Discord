using LundBot.Entities;

namespace LundBot.Repositories
{
    public abstract class AbstractMessageRepository<TEntity>
    {
        public abstract Task CreateAsync(TEntity entity);
        public abstract Task UpdateAsync(TEntity entity);
        public abstract Task DeleteManyAsync(IEnumerable<int> ids);
    }
}
