using System.ComponentModel;
using DSharpPlus.Commands;
using LundBot.Application.Features.WebsiteTraffic;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Commands.Checks;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.WebsiteTraffic
{
    public class RemoveWebsiteTrafficChannelCommand : AbstractBaseCommand
    {
        private readonly IWebsiteTrafficService _websiteTrafficService;

        public RemoveWebsiteTrafficChannelCommand(
            IDiscordInteractionService discordInteractionService,
            IWebsiteTrafficService websiteTrafficService
        )
            : base(discordInteractionService)
        {
            _websiteTrafficService = websiteTrafficService;
        }

        [RequireGuildOwner]
        [Command("remove-website-traffic-channel")]
        [Description("Removes the existing website traffic channel on this server.")]
        public async Task RemoveWebsiteTrafficChannelAsync(
            CommandContext context,
            [Parameter("confirm")] [Description("Confirm the removal of the leaderboard.")] bool confirm
        )
        {
            if (!await IsCommandSentFromGuild(context))
            {
                return;
            }

            if (!confirm)
            {
                await SendResponseAsync(
                    context,
                    "You must confirm the removal of the leaderboard by setting the 'Confirm' option to true."
                );
                return;
            }

            await TaskWithErrorHandlingAsync(
                context,
                () => _websiteTrafficService.RemoveWebsiteTrafficChannelAsync(context!.Guild!.Id),
                "Successfully removed the website traffic channel."
            );
        }
    }
}
