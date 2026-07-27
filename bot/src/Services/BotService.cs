using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Interfaces.Services;

namespace LundBot.Services
{
    public sealed class BotService : IBotService
    {
        public static DiscordClient DiscordClient { get; set; } = null!;

        public async Task InitializeAsync(DiscordClient discordClient)
        {
            await SetBotStatusAsync();
        }

        private async Task SetBotStatusAsync()
        {
            var activity = new DiscordActivity("Stuck in a movie theater", ActivityType.Playing);
            await DiscordClient.UpdateStatusAsync(activity, UserStatus.Online);
        }
    }
}
