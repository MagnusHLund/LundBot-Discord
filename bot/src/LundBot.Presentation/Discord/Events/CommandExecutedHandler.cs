using DSharpPlus;
using DSharpPlus.Commands.EventArgs;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class CommandExecutedHandler : IEventHandler<CommandExecutedEventArgs>
    {
        private readonly ILogger _logger = Log.ForContext<CommandExecutedHandler>();

        public async Task HandleEventAsync(DiscordClient sender, CommandExecutedEventArgs eventArgs)
        {
            _logger.Information(
                "Command invoked: {Cmd} by {User} in Guild={Guild}",
                eventArgs.Context.Command?.Name,
                eventArgs.Context.User?.Username,
                eventArgs.Context.Guild?.Id ?? 0
            );
        }
    }
}
