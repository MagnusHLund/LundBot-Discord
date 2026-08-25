using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Services.Discord.Events
{
    public class GuildMemberAddedHandler : IEventHandler<GuildMemberAddedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, GuildMemberAddedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
