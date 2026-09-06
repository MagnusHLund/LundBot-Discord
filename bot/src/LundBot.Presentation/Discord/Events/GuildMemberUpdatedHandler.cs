using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildMemberUpdatedHandler : IEventHandler<GuildMemberUpdatedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, GuildMemberUpdatedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
