using DSharpPlus;
using DSharpPlus.Commands.EventArgs;

namespace LundBot.Infrastructure.Discord.Events
{
    public sealed class CommandExecutedHandler : IEventHandler<CommandExecutedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, CommandExecutedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
