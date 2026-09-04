namespace LundBot.Application.Common.Discord
{
    public interface IDiscordBotService
    {
        Task ConnectToDiscordAsync();
        Task UpdateBotStatusAsync();
    }
}
