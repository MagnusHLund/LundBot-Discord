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
        private readonly IMessageService<
            WebsiteTrafficAnalyticsMessage,
            IWebsiteTrafficMessageRepository,
            WebsiteTrafficMessageFactory
        > _messageService;
        private readonly WebsiteTrafficConfig _discordConfig;

        public WebsiteTrafficService(
            IWebsiteTrafficRepository websiteTrafficRepository,
            IWebsiteTrafficMessageRepository websiteTrafficMessageRepository,
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
            _discordConfig = discordConfig.Value;
            _messageService = messageService;
        }

        public async Task<bool> RegisterWebsiteVisitAsync(string ipAddress)
        {
            byte[] hashedIpAddress = HashUtils.HashString(ipAddress);
            bool success = await _websiteTrafficRepository.RegisterWebsiteVisitAsync(hashedIpAddress);

            if (!success)
            {
                return false;
            }

            await UpdateWebsiteStatsMessageAsync();

            return true;
        }

        public async Task<bool> RegisterInviteLinkClickAsync(string ipAddress)
        {
            byte[] hashedIpAddress = HashUtils.HashString(ipAddress);
            bool success = await _websiteTrafficRepository.RegisterInviteLinkClickAsync(hashedIpAddress);

            if (!success)
            {
                return false;
            }

            await UpdateWebsiteStatsMessageAsync();

            return true;
        }

        private async Task UpdateWebsiteStatsMessageAsync()
        {
            string message = await GenerateWebsiteStatsMessageAsync();

            (DateTime startOfWeek, DateTime endOfWeek) = TimeUtils.GetCurrentUtcWeekBounds();

            List<WebsiteTrafficAnalyticsMessage> existingMessages =
                await _websiteTrafficMessageRepository.GetWebsiteTrafficMessagesForPeriodAsync(startOfWeek, endOfWeek);

            // TODO: This could get set with a owner-only command and stored in database. Could also be cached per guild.
            ulong channelId = _discordConfig.WebTrafficChannelId;

            await _messageService.SynchronizeDiscordMessagesAsync(message, existingMessages, channelId);
        }

        private async Task<string> GenerateWebsiteStatsMessageAsync()
        {
            (DateTime startOfWeek, DateTime endOfWeek) = TimeUtils.GetCurrentUtcWeekBounds();

            List<WebsiteTrafficAnalytics> websiteTrafficEntities =
                await _websiteTrafficRepository.GetWebsiteTrafficEntitiesForPeriodAsync(startOfWeek, endOfWeek);

            List<WebsiteTrafficAnalyticsMessage> websiteTrafficMessagesEntities =
                await _websiteTrafficMessageRepository.GetWebsiteTrafficMessagesForPeriodAsync(startOfWeek, endOfWeek);

            int totalVisits = websiteTrafficEntities.Count;
            int totalInviteClicks = websiteTrafficEntities.Count(w => w.ClickedInviteButton);

            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.AppendLine("# Website Traffic");
            messageBuilder.AppendLine($"Week (UTC): {startOfWeek:dd-MM-yyyy} to {endOfWeek:dd-MM-yyyy}");
            messageBuilder.AppendLine($"Total Visits: {totalVisits}");
            messageBuilder.AppendLine($"Invite Clicks: {totalInviteClicks}");
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("## Entries");

            for (int i = 0; i < websiteTrafficEntities.Count; i++)
            {
                var traffic = websiteTrafficEntities[i];
                string clickedInvite = traffic.ClickedInviteButton ? "✔️" : "❌";

                messageBuilder.AppendLine(
                    $"{i + 1}. {traffic.CreatedAt:dd-MM-yyyy HH:mm:ss} UTC | invite={clickedInvite}"
                );
            }

            return messageBuilder.ToString().Trim();
        }
    }
}
