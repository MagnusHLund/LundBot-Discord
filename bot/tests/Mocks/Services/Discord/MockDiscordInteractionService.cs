using DSharpPlus.Commands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LundBot.Interfaces.Services.Discord;

namespace LundBot.Tests.Mocks.Services.Discord;

internal sealed class MockDiscordInteractionService : IDiscordInteractionService
{
    internal bool IsCommandSentFromServerResult { get; set; } = true;
    internal List<(string Content, bool ShowOnlyToUser)> Responses { get; } = [];

    public ValueTask<bool> IsCommandSentFromServer(CommandContext context) =>
        ValueTask.FromResult(IsCommandSentFromServerResult);

    public Task SendResponseAsync(
        CommandContext context,
        string content,
        bool showOnlyToUser = true
    )
    {
        Responses.Add((content, showOnlyToUser));
        return Task.CompletedTask;
    }

    public Task SendResponseAsync(
        DiscordInteraction interaction,
        string content,
        bool showOnlyToUser = true
    )
    {
        Responses.Add((content, showOnlyToUser));
        return Task.CompletedTask;
    }

    public Task SendResponseAsync(
        DiscordInteraction interaction,
        DiscordInteractionResponseBuilder responseBuilder,
        bool showOnlyToUser = true
    ) => Task.CompletedTask;

    public Task HandleComponentInteractionAsync(ComponentInteractionCreatedEventArgs e) =>
        Task.CompletedTask;
}
