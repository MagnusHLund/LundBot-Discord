using LundBot.Data;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySqlConnector;

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

        public override Task CreateAsync(WelcomeMessageEntity entity)
        {
            try
            {
                _context.WelcomeMessages.Add(entity);
                return _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // TODO: Can this be written better? Keep in mind sqlite for automated testing.
                if (ex.InnerException is MySqlException mysqlEx)
                {
                    if (
                        mysqlEx.Message.Contains("Duplicate entry")
                        && mysqlEx.Message.Contains("for key 'IX_WelcomeMessages_DiscordUserId'")
                    )
                    {
                        _logger.Warning(
                            "Attempted to create a duplicate WelcomeMessageEntity for DiscordUserId: {DiscordUserId}",
                            entity.DiscordUserId
                        );
                        throw new InvalidOperationException(
                            $"A welcome message for DiscordUserId {entity.DiscordUserId} already exists.",
                            ex
                        );
                    }
                }
                else
                {
                    // TODO: Would this throw to the 2nd catch?
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating WelcomeMessageEntity: {Entity}", entity);
                throw new Exception("An error occurred while creating the entity.", ex);
            }

            throw new NotImplementedException(); // TODO: Better exception
        }

        public override Task DeleteManyAsync(IEnumerable<int> ids)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public Task<WelcomeMessageEntity> GetByJoinedUserIdAsync(string joinedUserId)
        {
            //  TODO: Implement
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(WelcomeMessageEntity entity)
        {
            // Does not need implementation for WelcomeMessageEntity
            throw new NotImplementedException();
        }
    }
}
