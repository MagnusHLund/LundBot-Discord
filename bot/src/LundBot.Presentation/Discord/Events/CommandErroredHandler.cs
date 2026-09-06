using DSharpPlus;
using DSharpPlus.Commands.EventArgs;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class CommandErroredHandler : IEventHandler<CommandErroredEventArgs>
    {
        private readonly IDiscordInteractionService _discordInteractionService;

        private readonly ILogger _logger = Log.ForContext<CommandErroredHandler>();

        public CommandErroredHandler(IDiscordInteractionService discordInteractionService)
        {
            _discordInteractionService = discordInteractionService;
        }

        public async Task HandleEventAsync(DiscordClient sender, CommandErroredEventArgs eventArgs)
        {
            _logger.Error(
                eventArgs.Exception,
                "Command errored: {Cmd} by {User} in Guild={Guild}",
                eventArgs.Context?.Command.Name ?? "<unknown>",
                eventArgs.Context?.User?.Username ?? "<unknown>",
                eventArgs.Context?.Guild?.Id ?? 0
            );

            try
            {
                if (eventArgs.Context != null)
                {
                    bool success = await _discordInteractionService.SendResponseAsync(
                        eventArgs.Context,
                        "Internal server error. Please try again later.",
                        showOnlyToUser: true
                    );

                    if (!success)
                    {
                        _logger.Warning("Failed to send error response to the user.");
                    }
                }
                else
                {
                    _logger.Warning("Cannot send error response because the context is null.");
                }
            }
            catch { }
        }
    }
}
