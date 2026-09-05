namespace LundBot.Application.Discord.Commands
{
    public interface IDiscordCommandService
    {
        Task RefreshCommandsAsync();
    }
}
