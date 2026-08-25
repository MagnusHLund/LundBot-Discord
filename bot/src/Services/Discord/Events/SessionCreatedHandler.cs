using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Services.Discord.Events
{
    public class SessionCreatedHandler : IEventHandler<SessionCreatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, SessionCreatedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
