using LundBot.Data;
using LundBot.Entities;
using LundBot.Enums;
using LundBot.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LundBot.Repositories
{
    public class LeaderboardsRepository : ILeaderboardsRepository
    {
        private readonly LundBotDiscordDbContext _context;
        private readonly Serilog.ILogger _logger = Log.ForContext<WebsiteTrafficRepository>();

        public LeaderboardsRepository(LundBotDiscordDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DoesLeaderboardExistAsync(string channelId, string guildId)
        {
            try
            {
                return await _context.Leaderboards.AnyAsync(l =>
                    l.DiscordChannelId == channelId && l.DiscordServerId == guildId
                );
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error checking if leaderboard exists for channel ID: {ChannelId} and guild ID: {GuildId}",
                    channelId,
                    guildId
                );
                return false;
            }
        }

        public async Task<LeaderboardsEntity> CreateLeaderboardAsync(
            string channelId,
            string guildId,
            string title,
            string message,
            LeaderboardType leaderboardType
        )
        {
            var leaderboard = new LeaderboardsEntity
            {
                DiscordChannelId = channelId,
                DiscordServerId = guildId,
                Title = title,
                Message = message,
                LeaderboardType = leaderboardType,
            };

            try
            {
                _context.Leaderboards.Add(leaderboard);
                await _context.SaveChangesAsync();
                return leaderboard;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error creating leaderboard for channel ID: {ChannelId} and guild ID: {GuildId}",
                    channelId,
                    guildId
                );
                throw new Exception("An error occurred while creating the leaderboard.", ex);
            }
        }
    }
}
