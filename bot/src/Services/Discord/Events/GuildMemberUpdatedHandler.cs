using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Services.Discord.Events
{
    public class GuildMemberUpdatedHandler : IEventHandler<GuildMemberUpdatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, GuildMemberUpdatedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
