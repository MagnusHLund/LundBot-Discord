using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Presentation.Discord.Events
{
    public sealed class GuildDownloadCompletedHandler : IEventHandler<GuildDownloadCompletedEventArgs>
    {
        public Task HandleEventAsync(DiscordClient sender, GuildDownloadCompletedEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }
    }
}
