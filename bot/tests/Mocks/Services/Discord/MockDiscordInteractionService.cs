using DSharpPlus.SlashCommands;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Tests.Mocks.Services.Discord;

public sealed class MockDiscordInteractionService : IDiscordInteractionService
{
    public bool IsCommandSentFromServerResult { get; set; } = true;
    public List<(string Content, bool ShowOnlyToUser)> Responses { get; } = [];

    public ValueTask<bool> IsCommandSentFromServer(InteractionContext context) =>
        ValueTask.FromResult(IsCommandSentFromServerResult);

    public Task SendResponseAsync(
        InteractionContext context,
        string content,
        bool showOnlyToUser = true
    )
    {
        Responses.Add((content, showOnlyToUser));
        return Task.CompletedTask;
    }
}
