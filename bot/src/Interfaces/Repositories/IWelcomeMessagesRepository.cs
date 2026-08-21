using LundBot.Entities;

namespace LundBot.Interfaces.Repositories
{
    public interface IWelcomeMessagesRepository
    {
        Task CreateAsync(WelcomeMessageEntity entity);
        Task<WelcomeMessageEntity> GetByJoinedUserIdAsync(string joinedUserId);
    }
}
