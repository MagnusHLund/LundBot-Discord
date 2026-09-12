using System.Text;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Common.Security.Hashing;
using LundBot.Application.Common.Time;
using LundBot.Domain.WebsiteTraffic;
using Microsoft.Extensions.Options;

namespace LundBot.Application.Features.WebsiteTraffic
{
    public sealed class WebsiteTrafficService : IWebsiteTrafficService
    {
        private readonly IWebsiteTrafficRepository _websiteTrafficRepository;
        private readonly IWebsiteTrafficMessageRepository _websiteTrafficMessageRepository;
        private readonly IWebsiteTrafficChannelRepository _websiteTrafficChannelRepository;
        private readonly IMessageService<
            WebsiteTrafficAnalyticsMessage,
            IWebsiteTrafficMessageRepository,
            WebsiteTrafficMessageFactory
        > _messageService;

        private readonly ILogger _logger = Log.ForContext<WebsiteTrafficService>();

        public WebsiteTrafficService(
            IWebsiteTrafficRepository websiteTrafficRepository,
            IWebsiteTrafficMessageRepository websiteTrafficMessageRepository,
            IWebsiteTrafficChannelRepository websiteTrafficChannelRepository,
            IOptions<WebsiteTrafficConfig> discordConfig,
            IMessageService<
                WebsiteTrafficAnalyticsMessage,
                IWebsiteTrafficMessageRepository,
                WebsiteTrafficMessageFactory
            > messageService
        )
        {
            _websiteTrafficRepository = websiteTrafficRepository;
            _websiteTrafficMessageRepository = websiteTrafficMessageRepository;
            _websiteTrafficChannelRepository = websiteTrafficChannelRepository;
            _messageService = messageService;
        }

        public async Task<bool> CreateWebsiteTrafficChannelAsync(ulong channelId, ulong guildId)
        {
            var channel = await _websiteTrafficChannelRepository.CreateWebsiteTrafficChannelAsync(channelId, guildId);
            return channel is not null;
        }

        public async Task<bool> RemoveWebsiteTrafficChannelAsync(ulong guildId)
        {
            return await _websiteTrafficChannelRepository.RemoveWebsiteTrafficChannelAsync(guildId);
        }

        public async Task<bool> RegisterWebsiteVisitAsync(string ipAddress, ulong guildId)
        {
            byte[] hashedIpAddress = HashUtils.HashString(ipAddress);
            bool success = await _websiteTrafficRepository.RegisterWebsiteVisitAsync(hashedIpAddress);

            if (!success)
            {
                return false;
            }

            await UpdateWebsiteStatsMessageAsync(guildId);

            return true;
        }

        public async Task<bool> RegisterInviteLinkClickAsync(string ipAddress, ulong guildId)
        {
            byte[] hashedIpAddress = HashUtils.HashString(ipAddress);
            bool success = await _websiteTrafficRepository.RegisterInviteLinkClickAsync(hashedIpAddress);

            if (!success)
            {
                return false;
            }

            await UpdateWebsiteStatsMessageAsync(guildId);

            return true;
        }

        private async Task UpdateWebsiteStatsMessageAsync(ulong guildId)
        {
            string message = await GenerateWebsiteStatsMessageAsync();

            (DateTime startOfWeek, DateTime endOfWeek) = TimeUtils.GetCurrentUtcWeekBounds();

            List<WebsiteTrafficAnalyticsMessage> existingMessages =
                await _websiteTrafficMessageRepository.GetWebsiteTrafficMessagesForPeriodAsync(startOfWeek, endOfWeek);

            WebsiteTrafficAnalyticsChannel? channel =
                await _websiteTrafficChannelRepository.GetWebsiteTrafficChannelAsync(guildId);

            if (channel is null)
            {
                _logger.Warning("Website traffic channel not found for guild ID: {GuildId}", guildId);
                return;
            }

            ulong channelId = channel.ChannelId;

            await _messageService.SynchronizeDiscordMessagesAsync(message, existingMessages, channelId);
        }

        private async Task<string> GenerateWebsiteStatsMessageAsync()
        {
            (DateTime startOfWeek, DateTime endOfWeek) = TimeUtils.GetCurrentUtcWeekBounds();

            List<WebsiteTrafficAnalytics> websiteTrafficEntities =
                await _websiteTrafficRepository.GetWebsiteTrafficEntitiesForPeriodAsync(startOfWeek, endOfWeek);

            int totalVisits = websiteTrafficEntities.Count;
            int totalInviteClicks = websiteTrafficEntities.Count(w => w.ClickedInviteButton);

            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.AppendLine("# Website Traffic");
            messageBuilder.AppendLine($"Week (UTC): {startOfWeek:dd-MM-yyyy} to {endOfWeek:dd-MM-yyyy}");
            messageBuilder.AppendLine($"Total Visits: {totalVisits}");
            messageBuilder.AppendLine($"Invite Clicks: {totalInviteClicks}");
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("## Entries");
            messageBuilder.AppendLine("```text");
            messageBuilder.AppendLine("#   Created At               | Invite");

            for (int i = 0; i < websiteTrafficEntities.Count; i++)
            {
                var traffic = websiteTrafficEntities[i];
                string clickedInvite = traffic.ClickedInviteButton ? "✔️" : "❌";

                messageBuilder.AppendLine(
                    $"{i + 1, -3} {traffic.CreatedAt:dd-MM-yyyy HH:mm:ss} UTC  | {clickedInvite}"
                );
            }

            messageBuilder.AppendLine("```");

            return messageBuilder.ToString().Trim();
        }
    }
}
