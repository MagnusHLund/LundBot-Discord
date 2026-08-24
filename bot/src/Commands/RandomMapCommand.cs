using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using LundBot.Interfaces.Services.Discord;
using LundBot.Utils;

namespace LundBot.Commands
{
    public class RandomMapCommand : BaseCommand
    {
        public RandomMapCommand(IDiscordInteractionService discordInteractionService)
            : base(discordInteractionService) { }

        [SlashRequirePermissions(true, DiscordPermission.Administrator)]
        [SlashCommand("random-map", "Selects a random map from the list of maps.")]
        public async Task RandomMapAsync(InteractionContext context)
        {
            var maps = Enum.GetNames(typeof(Enums.InfiniteWarfareZombiesMaps)).ToList();

            var random = new Random();
            string randomMap = maps[random.Next(maps.Count)];

            string formattedMap = StringUtils.SplitCamelCaseOrPascalCaseToWords(randomMap);

            await SendResponseAsync(context, $"{formattedMap}");
        }
    }
}
