using LundBot.Entities;
using LundBot.Services;
using LundBot.Tests.Mocks.Discord;
using LundBot.Tests.Mocks.Factories;
using LundBot.Tests.Mocks.Repositories;
using LundBot.Tests.Mocks.Services.Discord;
using LundBot.Tests.TestHelpers;
using Xunit;

namespace LundBot.Tests.Unit.Services
{
    public sealed class MessageServiceTests
    {
        private MessageService<
            LeaderboardMessagesEntity,
            MockMessageRepository<LeaderboardMessagesEntity>,
            MockMessageFactory<LeaderboardMessagesEntity>
        > CreateService(
            MockDiscordMessageService discordMessageService,
            MockDiscordChannelService discordChannelService,
            MockMessageRepository<LeaderboardMessagesEntity> repo
        )
        {
            return new MessageService<
                LeaderboardMessagesEntity,
                MockMessageRepository<LeaderboardMessagesEntity>,
                MockMessageFactory<LeaderboardMessagesEntity>
            >(
                repo,
                new MockMessageFactory<LeaderboardMessagesEntity>(),
                discordChannelService,
                discordMessageService
            );
        }

        [Fact]
        internal async Task SynchronizeDiscordMessagesAsync_UpdatesExistingMessages()
        {
            var repo = new MockMessageRepository<LeaderboardMessagesEntity>();
            var discord = new MockDiscordMessageService();
            var channelService = new MockDiscordChannelService();

            discord.GetMessageBehavior = id =>
                DiscordTestHelper.TestMessage(id, channelService.Channel);

            var existing = new List<LeaderboardMessagesEntity>
            {
                new() { Id = 1, DiscordMessageId = "10" },
            };

            var service = CreateService(discord, channelService, repo);

            await service.SynchronizeDiscordMessagesAsync(
                "Updated text",
                existing,
                channelService.Channel.Id
            );

            Assert.Single(discord.Modified);
            Assert.Equal("Updated text", discord.Modified[0].NewContent);
        }

        [Fact]
        internal async Task SynchronizeDiscordMessagesAsync_CreatesNewMessages()
        {
            var repo = new MockMessageRepository<LeaderboardMessagesEntity>();
            var discord = new MockDiscordMessageService();
            var channelService = new MockDiscordChannelService();

            discord.GetMessageBehavior = id =>
                DiscordTestHelper.TestMessage(id, channelService.Channel);

            var existing = new List<LeaderboardMessagesEntity>(); // empty

            var service = CreateService(discord, channelService, repo);

            await service.SynchronizeDiscordMessagesAsync(
                "Hello world",
                existing,
                channelService.Channel.Id
            );

            Assert.Single(discord.Sent);
            Assert.Single(repo.Created);
        }

        [Fact]
        internal async Task SynchronizeDiscordMessagesAsync_DeletesExtraMessages()
        {
            var repo = new MockMessageRepository<LeaderboardMessagesEntity>();
            var discord = new MockDiscordMessageService();
            var channelService = new MockDiscordChannelService();

            discord.GetMessageBehavior = id =>
                DiscordTestHelper.TestMessage(id, channelService.Channel);

            var existing = new List<LeaderboardMessagesEntity>
            {
                new() { Id = 1, DiscordMessageId = "10" },
                new() { Id = 2, DiscordMessageId = "11" },
            };

            var service = CreateService(discord, channelService, repo);

            await service.SynchronizeDiscordMessagesAsync(
                "Only one chunk",
                existing,
                channelService.Channel.Id
            );

            Assert.Single(discord.Deleted);
            Assert.Single(repo.Deleted);
            Assert.Equal(2, repo.Deleted[0]);
        }
    }
}
