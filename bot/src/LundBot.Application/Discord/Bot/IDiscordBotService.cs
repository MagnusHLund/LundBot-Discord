namespace LundBot.Application.Discord.Bot
{
    public interface IDiscordBotService
    {
        Task ConnectToDiscordAsync();
        Task UpdateBotStatusAsync();
    }
}
