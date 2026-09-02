using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Infrastructure.Discord.Events
{
    public sealed class SessionCreatedHandler : IEventHandler<SessionCreatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, SessionCreatedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
