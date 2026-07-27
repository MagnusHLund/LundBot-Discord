using System.Text;
using LundBot.Entities;
using LundBot.Interfaces.Repositories;
using LundBot.Interfaces.Services;
using LundBot.Utils;

namespace LundBot.Services
{
    public sealed class WebsiteTrafficService : IWebsiteTrafficService
    {
        private readonly IMessageService _messageService;
        private readonly IWebsiteTrafficRepository _websiteTrafficRepository;
        private readonly IWebsiteTrafficMessagesRepository _websiteTrafficMessagesRepository;

        public WebsiteTrafficService(
            IMessageService messageService,
            IWebsiteTrafficRepository websiteTrafficRepository,
            IWebsiteTrafficMessagesRepository websiteTrafficMessagesRepository
        )
        {
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

            await _messageService.SynchronizeWebsiteTrafficMessagesAsync(
                message,
                existingMessages,
                BotService.DiscordClient
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
                string clickedInvite = traffic.ClickedInviteButton ? "Yes" : "No";

                messageBuilder.AppendLine(
                    $"{i + 1}. {traffic.CreatedAt:dd-MM-yyyy HH:mm:ss} UTC | invite={clickedInvite}"
                );
            }

            return messageBuilder.ToString().Trim();
        }
    }
}
