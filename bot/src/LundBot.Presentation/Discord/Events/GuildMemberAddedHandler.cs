using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildMemberAddedHandler : IEventHandler<GuildMemberAddedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, GuildMemberAddedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
