using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Commands
{
    public sealed class PingCommand : BaseCommand
    {
        public PingCommand(IDiscordInteractionService discordInteractionService)
            : base(discordInteractionService) { }

        [SlashRequirePermissions(true, DiscordPermission.Administrator)]
        [SlashCommand("ping", "Pings the bot.")]
        public async Task PingAsync(InteractionContext context)
        {
            await SendResponseAsync(context, "Pong!");
        }
    }
}
