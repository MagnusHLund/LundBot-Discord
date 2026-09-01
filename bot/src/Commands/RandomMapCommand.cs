using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using LundBot.Utils;

namespace LundBot.Commands
{
    public class RandomMapCommand : BaseCommand
    {
        public RandomMapCommand(IDiscordInteractionService discordInteractionService)
            : base(discordInteractionService) { }

        [Command("random-map")]
        [Description("Selects a random map from the list of maps.")]
        public async Task RandomMapAsync(CommandContext context)
        {
            var maps = Enum.GetNames(typeof(Enums.InfiniteWarfareZombiesMaps)).ToList();

            var random = new Random();
            string randomMap = maps[random.Next(maps.Count)];

            string formattedMap = StringUtils.SplitCamelCaseOrPascalCaseToWords(randomMap);

            await SendResponseAsync(context, $"{formattedMap}");
        }
    }
}
