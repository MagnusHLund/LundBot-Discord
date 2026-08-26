using DSharpPlus;
using DSharpPlus.Commands.EventArgs;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Services.Discord.Events
{
    public class CommandErroredHandler : IEventHandler<CommandErroredEventArgs>
    {
        private readonly IDiscordInteractionService _discordInteractionService;
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<CommandErroredHandler>();

        public CommandErroredHandler(IDiscordInteractionService discordInteractionService)
        {
            _discordInteractionService = discordInteractionService;
        }

        public async Task HandleEventAsync(DiscordClient sender, CommandErroredEventArgs e)
        {
            _logger.Error(
                e.Exception,
                "Slash errored: {Cmd} by {User} in Guild={Guild}",
                e.Context?.Command.Name ?? "<unknown>",
                e.Context?.User?.Username ?? "<unknown>",
                e.Context?.Guild?.Id ?? 0
            );

            try
            {
                if (e.Context != null)
                {
                    await _discordInteractionService.SendResponseAsync(
                        e.Context,
                        "Internal server error. Please try again later.",
                        showOnlyToUser: true
                    );
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
