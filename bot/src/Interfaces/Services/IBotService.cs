using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

namespace LundBot.Interfaces.Services
{
    public interface IBotService
    {
        static DiscordClient DiscordClient { get; set; } = null!;
        Task InitializeAsync(DiscordClient discordClient);
        Task OnGuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e);
        Task OnGuildMemberAdded(DiscordClient sender, GuildMemberAddedEventArgs e);
        Task OnGuildMemberUpdated(DiscordClient sender, GuildMemberUpdatedEventArgs e);
        Task OnGuildCreated(DiscordClient sender, GuildCreatedEventArgs e);
        Task OnSlashCommandExecuted(SlashCommandsExtension sender, SlashCommandExecutedEventArgs e);
        Task OnSlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e);
        Task OnClientReady(DiscordClient sender, SessionCreatedEventArgs e);
        Task OnComponentInteractionCreated(
            DiscordClient sender,
            ComponentInteractionCreatedEventArgs e
        );
    }
}
