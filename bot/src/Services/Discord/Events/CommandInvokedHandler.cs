using DSharpPlus;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.EventArgs;

namespace LundBot.Services.Discord.Events
{
    public class CommandInvokedHandler : IEventHandler<CommandExecutedEventArgs>
    {
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<CommandInvokedHandler>();

        public async Task HandleEventAsync(DiscordClient sender, CommandExecutedEventArgs e)
        {
            _logger.Information(
                "Command invoked: {Cmd} by {User} in Guild={Guild}",
                e.Context.Command?.Name,
                e.Context.User?.Username,
                e.Context.Guild?.Id ?? 0
            );
        }
    }
}
