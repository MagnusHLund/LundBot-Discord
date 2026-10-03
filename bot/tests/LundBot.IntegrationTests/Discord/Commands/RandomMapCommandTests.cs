using DSharpPlus.Commands;
using LundBot.Application.Features.InfiniteWarfare.Maps;
using LundBot.Presentation.Discord.InfiniteWarfare.Maps;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord.Commands;

public sealed class RandomMapCommandTests
{
    [Fact]
    public async Task RandomMap_sends_selected_map_privately()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        IRandomMapService randomMapService = Substitute.For<IRandomMapService>();
        randomMapService.GetRandomMap().Returns("Shipment");
        RandomMapCommand command = new RandomMapCommand(interactionService, randomMapService);

        await command.RandomMapAsync(context);

        await interactionService.Received(1).SendResponseAsync(Arg.Any<CommandContext>(), "Random map: Shipment", true);
    }

    [Fact]
    public async Task RandomMap_reports_safe_error_privately_when_service_throws()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        IRandomMapService randomMapService = Substitute.For<IRandomMapService>();
        randomMapService.GetRandomMap().Returns(_ => throw new InvalidOperationException("Map unavailable"));
        RandomMapCommand command = new RandomMapCommand(interactionService, randomMapService);

        await command.RandomMapAsync(context);

        await interactionService
            .Received(1)
            .SendResponseAsync(Arg.Any<CommandContext>(), "An error occurred while selecting a random map.", true);
    }
}
