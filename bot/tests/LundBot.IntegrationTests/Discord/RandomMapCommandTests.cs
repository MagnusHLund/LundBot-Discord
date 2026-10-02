using DSharpPlus.Commands;
using LundBot.Application.Features.InfiniteWarfare.Maps;
using LundBot.Presentation.Discord.InfiniteWarfare.Maps;
using NSubstitute;
using Xunit;

namespace LundBot.IntegrationTests.Discord;

public sealed class RandomMapCommandTests
{
    [Fact]
    public async Task RandomMap_sends_selected_map()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        IRandomMapService randomMapService = Substitute.For<IRandomMapService>();
        randomMapService.GetRandomMap().Returns("Shipment");
        var command = new RandomMapCommand(interactionService, randomMapService);

        await command.RandomMapAsync(context);

        await interactionService
            .Received(1)
            .SendResponseAsync(Arg.Any<CommandContext>(), "Random map: Shipment", false);
    }

    [Fact]
    public async Task RandomMap_reports_service_exception()
    {
        CommandContext context = DiscordCommandTestDoubles.CreateContext();
        var interactionService = DiscordCommandTestDoubles.CreateInteractionService();
        IRandomMapService randomMapService = Substitute.For<IRandomMapService>();
        randomMapService.GetRandomMap().Returns(_ => throw new InvalidOperationException("Map unavailable"));
        RandomMapCommand command = new RandomMapCommand(interactionService, randomMapService);

        await command.RandomMapAsync(context);

        await interactionService
            .Received(1)
            .SendResponseAsync(Arg.Any<CommandContext>(), "An error occurred: Map unavailable", false);
    }
}
