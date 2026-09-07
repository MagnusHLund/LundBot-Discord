using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.Bot
{
    public sealed class PingCommand : AbstractBaseCommand
    {
        public PingCommand(IDiscordInteractionService discordInteractionService)
            : base(discordInteractionService) { }

        [Command("ping")]
        [Description("Pings the bot.")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task PingAsync(CommandContext context)
        {
            await SendResponseAsync(context, "Pong!");
        }
    }
}
