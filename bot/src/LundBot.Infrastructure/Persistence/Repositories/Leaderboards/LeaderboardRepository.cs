using LundBot.Application.Common.Exceptions;
using LundBot.Application.Features.Leaderboards.Contracts;
using LundBot.Domain.Leaderboards;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Infrastructure.Persistence.Repositories.Leaderboards
{
    public sealed class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<LeaderboardRepository>();

        public LeaderboardRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<(bool, Leaderboard?)> DoesLeaderboardExistAsync(ulong channelId, ulong guildId)
        {
            try
            {
                var leaderboard = await _context.Leaderboards.FirstOrDefaultAsync(l =>
                    l.DiscordChannelId == channelId && l.DiscordServerId == guildId
                );

                return (leaderboard != null, leaderboard);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error checking if leaderboard exists for channel ID: {ChannelId} and guild ID: {GuildId}",
                    channelId,
                    guildId
                );

                throw new RepositoryException(
                    $"Failed to check if a leaderboard exists for channel ID {channelId} and guild ID {guildId}.",
                    ex
                );
            }
        }

        public async Task<Leaderboard?> CreateLeaderboardAsync(
            ulong channelId,
            ulong guildId,
            string title,
            string message,
            LeaderboardTypeEnum leaderboardType
        )
        {
            Leaderboard leaderboard = new Leaderboard
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
                return null;
            }
        }

        public async Task<bool> RemoveLeaderboardAsync(ulong channelId, ulong guildId)
        {
            try
            {
                Leaderboard? leaderboard = await _context.Leaderboards.FirstOrDefaultAsync(l =>
                    l.DiscordChannelId == channelId && l.DiscordServerId == guildId
                );

                if (leaderboard is not null)
                {
                    _context.Leaderboards.Remove(leaderboard);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error removing leaderboard for channel ID: {ChannelId} and guild ID: {GuildId}",
                    channelId,
                    guildId
                );
                return false;
            }
        }

        public async Task<(bool, Leaderboard?)> DoesInviteLeaderboardExistOnServerAsync(ulong guildId)
        {
            try
            {
                Leaderboard? leaderboard = await _context.Leaderboards.FirstOrDefaultAsync(l =>
                    l.DiscordServerId == guildId && l.LeaderboardType == LeaderboardTypeEnum.Invite
                );
                return (leaderboard != null, leaderboard);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error checking if invite leaderboard exists for guild ID: {GuildId}", guildId);

                throw new RepositoryException(
                    $"Failed to check if an invite leaderboard exists for guild ID {guildId}.",
                    ex
                );
            }
        }

        public async Task<List<Leaderboard>> GetLeaderboardsForGuildAsync(ulong guildId)
        {
            try
            {
                return await _context.Leaderboards.Where(l => l.DiscordServerId == guildId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error retrieving leaderboards for guild ID: {GuildId}", guildId);

                throw new RepositoryException($"Failed to retrieve leaderboards for guild ID {guildId}.", ex);
            }
        }
    }
}
