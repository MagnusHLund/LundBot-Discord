namespace LundBot.Application.Common.Discord
{
    public interface IDiscordCommandService
    {
        Task RefreshCommandsAsync();
    }
}
