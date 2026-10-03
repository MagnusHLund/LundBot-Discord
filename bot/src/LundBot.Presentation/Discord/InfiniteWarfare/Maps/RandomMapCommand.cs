using System.ComponentModel;
using DSharpPlus.Commands;
using LundBot.Application.Features.InfiniteWarfare.Maps;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.InfiniteWarfare.Maps
{
    public sealed class RandomMapCommand : AbstractBaseCommand
    {
        private readonly IRandomMapService _randomMapService;

        private readonly ILogger _logger = Log.ForContext<RandomMapCommand>();

        public RandomMapCommand(
            IDiscordInteractionService discordInteractionService,
            IRandomMapService randomMapService
        )
            : base(discordInteractionService)
        {
            _randomMapService = randomMapService;
        }

        [Command("random-map")]
        [Description("Selects a random map from the list of maps.")]
        public async Task RandomMapAsync(CommandContext context)
        {
            string randomMap;

            try
            {
                randomMap = _randomMapService.GetRandomMap();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while selecting a random map.");
                await context.RespondAsync("An error occurred while selecting a random map.");
                return;
            }

            await context.RespondAsync($"Random map: {randomMap}");
        }
    }
}
