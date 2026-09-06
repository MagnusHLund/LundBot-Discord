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
            string randomMap = _randomMapService.GetRandomMap();

            await TaskWithErrorHandlingAsync(
                context,
                () => Task.FromResult(_randomMapService.GetRandomMap()),
                $"Random map: {randomMap}"
            );
        }
    }
}
