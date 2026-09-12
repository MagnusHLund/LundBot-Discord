using System.Text;
using LundBot.Application.Common.Messaging;
using LundBot.Application.Common.Security.Hashing;
using LundBot.Application.Common.Time;
using LundBot.Domain.WebsiteTraffic;

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
            WebsiteTrafficAnalyticsChannel? trafficChannel =
                await _websiteTrafficChannelRepository.GetWebsiteTrafficChannelAsync(guildId);

            if (trafficChannel is null)
            {
                _logger.Warning("Website traffic channel not found for guild ID: {GuildId}", guildId);
                return false;
            }

            byte[] hashedIpAddress = HashUtils.HashString(ipAddress);
            bool success = await _websiteTrafficRepository.RegisterWebsiteVisitAsync(
                hashedIpAddress,
                trafficChannel.Id
            );

            if (!success)
            {
                return false;
            }

            await UpdateWebsiteStatsMessageAsync(trafficChannel);

            return true;
        }

        public async Task<bool> RegisterInviteLinkClickAsync(string ipAddress, ulong guildId)
        {
            WebsiteTrafficAnalyticsChannel? trafficChannel =
                await _websiteTrafficChannelRepository.GetWebsiteTrafficChannelAsync(guildId);

            if (trafficChannel is null)
            {
                _logger.Warning("Website traffic channel not found for guild ID: {GuildId}", guildId);
                return false;
            }

            byte[] hashedIpAddress = HashUtils.HashString(ipAddress);
            bool success = await _websiteTrafficRepository.RegisterInviteLinkClickAsync(
                hashedIpAddress,
                trafficChannel.Id
            );

            if (!success)
            {
                return false;
            }

            await UpdateWebsiteStatsMessageAsync(trafficChannel);

            return true;
        }

        private async Task UpdateWebsiteStatsMessageAsync(WebsiteTrafficAnalyticsChannel trafficChannel)
        {
            string message = await GenerateWebsiteStatsMessageAsync(trafficChannel.Id);

            (DateTime startOfWeek, DateTime endOfWeek) = TimeUtils.GetCurrentUtcWeekBounds();

            List<WebsiteTrafficAnalyticsMessage> existingMessages =
                await _websiteTrafficMessageRepository.GetWebsiteTrafficMessagesForPeriodAsync(
                    startOfWeek,
                    endOfWeek,
                    trafficChannel.Id
                );

            _messageService.MessageFactory.SetWebsiteTrafficAnalyticsChannelId(trafficChannel.Id);

            ulong channelId = trafficChannel.ChannelId;

            await _messageService.SynchronizeDiscordMessagesAsync(message, existingMessages, channelId);
        }

        private async Task<string> GenerateWebsiteStatsMessageAsync(int websiteTrafficAnalyticsChannelId)
        {
            (DateTime startOfWeek, DateTime endOfWeek) = TimeUtils.GetCurrentUtcWeekBounds();

            List<WebsiteTrafficAnalytics> websiteTrafficEntities =
                await _websiteTrafficRepository.GetWebsiteTrafficEntitiesForPeriodAsync(
                    startOfWeek,
                    endOfWeek,
                    websiteTrafficAnalyticsChannelId
                );

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
