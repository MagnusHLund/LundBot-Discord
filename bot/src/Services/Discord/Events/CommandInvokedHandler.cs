using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Services.Discord.Events
{
    public class CommandInvokedHandler : IEventHandler<ComponentInteractionCreatedEventArgs>
    {
        private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<CommandInvokedHandler>();

        public async Task HandleEventAsync(
            DiscordClient sender,
            ComponentInteractionCreatedEventArgs e
        )
        {
            _logger.Information(
                "Command invoked: {Cmd} by {User} in Guild={Guild}",
                e.Id,
                e.User?.Username,
                e.Guild?.Id ?? 0
            );
        }
    }
}
