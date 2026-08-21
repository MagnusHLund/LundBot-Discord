using LundBot.Entities;
using LundBot.Repositories;

namespace LundBot.Tests.Mocks.Repositories;

internal sealed class MockMessageRepository<TEntity> : AbstractMessageRepository<TEntity>
    where TEntity : AbstractMessageEntity, new()
{
    internal List<TEntity> Created { get; } = new();
    internal List<TEntity> Updated { get; } = new();
    internal List<int> Deleted { get; } = new();

    public override Task CreateAsync(TEntity entity)
    {
        Created.Add(entity);
        return Task.CompletedTask;
    }

    public override Task UpdateAsync(TEntity entity)
    {
        Updated.Add(entity);
        return Task.CompletedTask;
    }

    public override Task DeleteManyAsync(IEnumerable<int> ids)
    {
        Deleted.AddRange(ids);
        return Task.CompletedTask;
    }
}
