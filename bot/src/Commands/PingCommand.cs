using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Commands
{
    public sealed class PingCommand : BaseCommand
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
