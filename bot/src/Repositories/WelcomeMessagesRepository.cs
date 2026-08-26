using LundBot.Data;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Repositories
{
    public class WelcomeMessagesRepository
        : AbstractMessageRepository<WelcomeMessageEntity>,
            IWelcomeMessagesRepository
    {
        private readonly LundBotDiscordDbContext _context;
        private readonly Serilog.ILogger _logger =
            Serilog.Log.ForContext<WelcomeMessagesRepository>();

        public WelcomeMessagesRepository(LundBotDiscordDbContext context)
        {
            _context = context;
        }

        public override async Task CreateAsync(WelcomeMessageEntity entity)
        {
            try
            {
                _context.WelcomeMessages.Add(entity);
                await _context.SaveChangesAsync();
                return;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating WelcomeMessageEntity: {Entity}", entity);
                throw new Exception("An error occurred while creating the entity.", ex);
            }
        }

        public override Task DeleteManyAsync(IEnumerable<int> ids)
        {
            var entitiesToDelete = _context.WelcomeMessages.Where(e => ids.Contains(e.Id));
            _context.WelcomeMessages.RemoveRange(entitiesToDelete);
            return _context.SaveChangesAsync();
        }

        public async Task<WelcomeMessageEntity> GetByJoinedUserIdAsync(string joinedUserId)
        {
            try
            {
                return await _context.WelcomeMessages.FirstOrDefaultAsync(e =>
                        e.DiscordUserId == joinedUserId
                    )
                    ?? throw new KeyNotFoundException(
                        $"No welcome message found for DiscordUserId {joinedUserId}."
                    );
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error retrieving WelcomeMessageEntity for DiscordUserId: {DiscordUserId}",
                    joinedUserId
                );
                throw new Exception(
                    $"An error occurred while retrieving the welcome message for DiscordUserId {joinedUserId}.",
                    ex
                );
            }
        }

        public override Task UpdateAsync(WelcomeMessageEntity entity)
        {
            // Does not need implementation for WelcomeMessageEntity
            throw new NotImplementedException();
        }
    }
}
