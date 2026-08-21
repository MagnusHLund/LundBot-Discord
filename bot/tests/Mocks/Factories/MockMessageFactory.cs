using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;

namespace LundBot.Tests.Mocks.Factories;

internal sealed class MockMessageFactory<TEntity> : IMessageEntityFactory<TEntity>
    where TEntity : AbstractMessageEntity, new()
{
    public TEntity Create(string discordMessageId) =>
        new TEntity { DiscordMessageId = discordMessageId };
}
