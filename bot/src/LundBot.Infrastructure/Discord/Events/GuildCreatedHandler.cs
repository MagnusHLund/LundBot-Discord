using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Infrastructure.Discord.Events
{
    public sealed class GuildCreatedHandler : IEventHandler<GuildCreatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, GuildCreatedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
