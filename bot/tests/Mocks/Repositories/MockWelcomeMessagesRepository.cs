using LundBot.Entities;
using LundBot.Interfaces.Repositories;

namespace LundBot.Tests.Mocks.Repositories;

internal sealed class MockWelcomeMessagesRepository : IWelcomeMessagesRepository
{
    internal List<WelcomeMessageEntity> Created { get; } = new();
    internal Func<string, WelcomeMessageEntity>? GetByJoinedUserIdBehavior { get; set; }

    public Task CreateAsync(WelcomeMessageEntity entity)
    {
        Created.Add(entity);
        return Task.CompletedTask;
    }

    public Task<WelcomeMessageEntity> GetByJoinedUserIdAsync(string joinedUserId)
    {
        if (GetByJoinedUserIdBehavior is null)
            throw new InvalidOperationException("MockWelcomeMessagesRepository.GetByJoinedUserIdBehavior not set.");

        return Task.FromResult(GetByJoinedUserIdBehavior(joinedUserId));
    }
}
