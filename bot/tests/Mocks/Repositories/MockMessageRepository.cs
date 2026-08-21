using LundBot.Entities;
using LundBot.Repositories;

namespace LundBot.Tests.Mocks.Repositories;

public sealed class MockMessageRepository<TEntity> : AbstractMessageRepository<TEntity>
    where TEntity : AbstractMessageEntity, new()
{
    public List<TEntity> Created { get; } = new();
    public List<TEntity> Updated { get; } = new();
    public List<int> Deleted { get; } = new();

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
