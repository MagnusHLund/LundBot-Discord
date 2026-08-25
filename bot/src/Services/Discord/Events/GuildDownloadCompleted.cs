using DSharpPlus;
using DSharpPlus.EventArgs;

namespace LundBot.Services.Discord.Events
{
    public class GuildDownloadCompletedHandler : IEventHandler<GuildDownloadCompletedEventArgs>
    {
        public Task HandleEventAsync(
            DiscordClient sender,
            GuildDownloadCompletedEventArgs eventArgs
        )
        {
            throw new NotImplementedException();
        }
    }
}
