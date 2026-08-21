namespace LundBot.Tests.Fakes.Discord
{
    internal sealed class FakeDiscordMessage
    {
        internal ulong Id { get; set; }
        internal FakeDiscordChannel Channel { get; set; } = default!;
    }
}
