using System.ComponentModel;
using DSharpPlus.Commands;
using LundBot.Infrastructure.Utils;
using LundBot.Presentation.Discord.Bot;
using LundBot.Presentation.Discord.Interactions;

namespace LundBot.Presentation.Discord.Games
{
    public sealed class RandomMapCommand : AbstractBaseCommand
    {
        public RandomMapCommand(IDiscordInteractionService discordInteractionService)
            : base(discordInteractionService) { }

        [Command("random-map")]
        [Description("Selects a random map from the list of maps.")]
        public async Task RandomMapAsync(CommandContext context)
        {
            // TODO: This should probably be moved to a service in the application layer.

            /*
            var maps = Enum.GetNames(typeof(Enums.InfiniteWarfareZombiesMapsEnum)).ToList();

            var random = new Random();
            string randomMap = maps[random.Next(maps.Count)];

            string formattedMap = StringUtils.SplitCamelCaseOrPascalCaseToWords(randomMap);

            await SendResponseAsync(context, $"{formattedMap}");
            */
        }
    }
}
