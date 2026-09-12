using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Domain.WebsiteTraffic;
using Microsoft.EntityFrameworkCore;

namespace LundBot.Infrastructure.Persistence.Repositories.WebsiteTraffic
{
    public class WebsiteTrafficChannelRepository : IWebsiteTrafficChannelRepository
    {
        private readonly LundBotDbContext _context;
        private readonly ILogger _logger = Log.ForContext<WebsiteTrafficChannelRepository>();

        public WebsiteTrafficChannelRepository(LundBotDbContext context)
        {
            _context = context;
        }

        public async Task<WebsiteTrafficAnalyticsChannel?> CreateWebsiteTrafficChannelAsync(
            ulong channelId,
            ulong guildId
        )
        {
            WebsiteTrafficAnalyticsChannel websiteTrafficAnalyticsChannel = new WebsiteTrafficAnalyticsChannel
            {
                ChannelId = channelId,
                GuildId = guildId,
            };

            try
            {
                _context.WebsiteTrafficAnalyticsChannels.Add(websiteTrafficAnalyticsChannel);
                await _context.SaveChangesAsync();
                return websiteTrafficAnalyticsChannel;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error creating website traffic analytics channel for channel ID: {ChannelId} and guild ID: {GuildId}",
                    channelId,
                    guildId
                );
                return null;
            }
        }

        public async Task<WebsiteTrafficAnalyticsChannel?> GetWebsiteTrafficChannelAsync(ulong guildId)
        {
            try
            {
                var entity = await _context.WebsiteTrafficAnalyticsChannels.FirstOrDefaultAsync(x =>
                    x.GuildId == guildId
                );

                return entity;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error retrieving website traffic analytics channel for guild ID: {GuildId}",
                    guildId
                );
                return null;
            }
        }

        public async Task<bool> RemoveWebsiteTrafficChannelAsync(ulong guildId)
        {
            try
            {
                var entity = await _context.WebsiteTrafficAnalyticsChannels.FirstOrDefaultAsync(x =>
                    x.GuildId == guildId
                );

                if (entity == null)
                {
                    return false;
                }

                _context.WebsiteTrafficAnalyticsChannels.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting website traffic analytics channel for guild ID: {GuildId}", guildId);
                return false;
            }
        }
    }
}
