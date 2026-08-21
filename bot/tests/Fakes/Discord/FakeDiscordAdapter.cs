using DSharpPlus.Entities;

namespace LundBot.Tests.Fakes.Discord
{
    internal sealed class FakeDiscordAdapter
    {
        internal static DiscordChannel ToReal(FakeDiscordChannel fake) => new DiscordChannel(); // never used, only needed for interface compatibility

        internal static DiscordMessage ToReal(FakeDiscordMessage fake) => new DiscordMessage(); // never used
    }
}
