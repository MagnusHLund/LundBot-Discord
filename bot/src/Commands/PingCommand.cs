using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace LundBot.Commands
{
    public sealed class PingCommand : BaseCommand
    {
        [SlashRequirePermissions(Permissions.Administrator)]
        [SlashCommand("ping", "Pings the bot.")]
        public async Task PingAsync(InteractionContext context)
        {
            await context.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Pong!")
            );
        }
    }
}
