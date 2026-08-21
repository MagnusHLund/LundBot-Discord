using LundBot.Entities;
using LundBot.Services;
using LundBot.Tests.Mocks.Factories;
using LundBot.Tests.Mocks.Repositories;
using LundBot.Tests.Mocks.Services.Discord;

namespace LundBot.Tests.Unit.Services.Factories;

internal static class MessageServiceTestFactory
{
    internal static MessageService<
        LeaderboardMessagesEntity,
        MockMessageRepository<LeaderboardMessagesEntity>,
        MockMessageFactory<LeaderboardMessagesEntity>
    > Create(
        MockDiscordMessageService discordMessageService,
        MockDiscordChannelService discordChannelService,
        MockMessageRepository<LeaderboardMessagesEntity> repo
    ) =>
        new(
            repo,
            new MockMessageFactory<LeaderboardMessagesEntity>(),
            discordChannelService,
            discordMessageService
        );
}
