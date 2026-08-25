using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.EventArgs;

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
        Task OnSlashCommandExecuted(CommandsExtension sender, CommandExecutedEventArgs e);
        Task OnSlashCommandErrored(CommandsExtension sender, CommandErroredEventArgs e);
        Task OnClientReady(DiscordClient sender, SessionCreatedEventArgs e);
        Task OnComponentInteractionCreated(
            DiscordClient sender,
            ComponentInteractionCreatedEventArgs e
        );
    }
}
