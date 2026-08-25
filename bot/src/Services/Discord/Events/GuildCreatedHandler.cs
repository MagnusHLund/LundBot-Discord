using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Services.Discord.Events
{
    public class GuildCreatedHandler : IEventHandler<GuildCreatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, GuildCreatedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
