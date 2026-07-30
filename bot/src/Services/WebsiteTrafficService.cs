using System.Text;
using LundBot.Config;
using LundBot.Entities;
using LundBot.Factories.MessageEntityFactories;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Repositories;
using LundBot.Utils;
using Microsoft.Extensions.Options;

namespace LundBot.Services
{
    public sealed class WebsiteTrafficService : IWebsiteTrafficService
    {
        private readonly DiscordConfig _discordConfig;
        private readonly IMessageService<
            WebsiteTrafficMessagesEntity,
            WebsiteTrafficMessagesRepository,
            WebsiteTrafficMessageFactory
        > _messageService;
        private readonly IWebsiteTrafficRepository _websiteTrafficRepository;
        private readonly WebsiteTrafficMessagesRepository _websiteTrafficMessagesRepository;

        public WebsiteTrafficService(
            IOptions<DiscordConfig> options,
            IMessageService<
                WebsiteTrafficMessagesEntity,
                WebsiteTrafficMessagesRepository,
                WebsiteTrafficMessageFactory
            > messageService,
            IWebsiteTrafficRepository websiteTrafficRepository,
            WebsiteTrafficMessagesRepository websiteTrafficMessagesRepository
        )
        {
            _discordConfig = options.Value;
            _messageService = messageService;
            _websiteTrafficRepository = websiteTrafficRepository;
            _websiteTrafficMessagesRepository = websiteTrafficMessagesRepository;
        }

        public async Task<bool> RegisterWebsiteVisitAsync(string ipAddress)
        {
            byte[] hashedIpAddress = HashUtils.HashString(ipAddress);
            bool success = await _websiteTrafficRepository.RegisterWebsiteVisitAsync(
                hashedIpAddress
            );

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
            bool success = await _websiteTrafficRepository.RegisterInviteLinkClickAsync(
                hashedIpAddress
            );

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

            (DateTime startOfWeek, DateTime endOfWeek) = TimeUtils.getCurrentUtcWeekBounds();

            List<WebsiteTrafficMessagesEntity> existingMessages =
                await _websiteTrafficMessagesRepository.GetWebsiteTrafficMessagesForPeriodAsync(
                    startOfWeek,
                    endOfWeek
                );

            ulong channelId = _discordConfig.WebTrafficChannelId;

            await _messageService.SynchronizeDiscordMessagesAsync(
                message,
                existingMessages,
                channelId
            );
        }

        private async Task<string> GenerateWebsiteStatsMessageAsync()
        {
            (DateTime startOfWeek, DateTime endOfWeek) = TimeUtils.getCurrentUtcWeekBounds();

            List<WebsiteTrafficEntity> websiteTrafficEntities =
                await _websiteTrafficRepository.GetWebsiteTrafficEntitiesForPeriodAsync(
                    startOfWeek,
                    endOfWeek
                );

            List<WebsiteTrafficMessagesEntity> websiteTrafficMessagesEntities =
                await _websiteTrafficMessagesRepository.GetWebsiteTrafficMessagesForPeriodAsync(
                    startOfWeek,
                    endOfWeek
                );

            int totalVisits = websiteTrafficEntities.Count;
            int totalInviteClicks = websiteTrafficEntities.Count(w => w.ClickedInviteButton);

            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.AppendLine("# Website Traffic");
            messageBuilder.AppendLine(
                $"Week (UTC): {startOfWeek:dd-MM-yyyy} to {endOfWeek:dd-MM-yyyy}"
            );
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
