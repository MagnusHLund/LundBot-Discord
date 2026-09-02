using DSharpPlus;
using DSharpPlus.Commands.EventArgs;

namespace LundBot.Infrastructure.Discord.Events
{
    public sealed class CommandErroredHandler : IEventHandler<CommandErroredEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, CommandErroredEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
