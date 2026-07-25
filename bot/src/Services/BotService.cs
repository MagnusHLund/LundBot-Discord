using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Interfaces.Services;

namespace LundBot.Services
{
    public sealed class BotService : IBotService
    {
        private readonly ICommandsService _commandsService;

        public BotService(ICommandsService commandsService)
        {
            _commandsService = commandsService;
        }

        public async Task InitializeAsync(DiscordClient discordClient)
        {
            await SetBotStatusAsync(discordClient);

            await _commandsService.ClearCommandsAsync(discordClient);
            await _commandsService.RegisterCommandsAsync(discordClient);
        }

        private async Task SetBotStatusAsync(DiscordClient discordClient)
        {
            var activity = new DiscordActivity("Stuck in a movie theater", ActivityType.Playing);
            await discordClient.UpdateStatusAsync(activity, UserStatus.Online);
        }
    }
}
