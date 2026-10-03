using Xunit;

namespace LundBot.IntegrationTests.Discord.Events.Support;

[CollectionDefinition("Discord events", DisableParallelization = true)]
public sealed class DiscordEventCollection;

[Collection("Discord events")]
public abstract class DiscordEventTestBase : IDisposable
{
    internal DiscordEventTestHost Host { get; } = new();

    public void Dispose() => Host.Dispose();
}
