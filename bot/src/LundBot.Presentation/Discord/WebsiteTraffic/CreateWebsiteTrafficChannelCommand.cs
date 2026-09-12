using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;
using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Commands.Checks;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.WebsiteTraffic
{
    public class CreateWebsiteTrafficChannelCommand : AbstractBaseCommand
    {
        [RequireGuildOwner]
        private readonly IWebsiteTrafficService _websiteTrafficService;

        public CreateWebsiteTrafficChannelCommand(
            IDiscordInteractionService discordInteractionService,
            IWebsiteTrafficService websiteTrafficService
        )
            : base(discordInteractionService)
        {
            _websiteTrafficService = websiteTrafficService;
        }

        [RequireGuildOwner]
        [Command("create-website-traffic-channel")]
        [Description("Creates a new website traffic channel on this server.")]
        public async Task CreateWebsiteTrafficChannelAsync(
            CommandContext context,
            [Parameter("channel")] [Description("The Channel that the leaderboard is in.")] DiscordChannel channel
        )
        {
            if (!await IsCommandSentFromGuild(context))
            {
                return;
            }

            if (!await IsValidDiscordIdAsync(context, channel.Id, "channel"))
            {
                return;
            }

            await TaskWithErrorHandlingAsync(
                context,
                () => _websiteTrafficService.CreateWebsiteTrafficChannelAsync(channel.Id, context!.Guild!.Id),
                "Successfully created the website traffic channel."
            );
        }
    }
}
