namespace LundBot.Application.Discord.Bot
{
    public interface IDiscordBotService
    {
        Task<bool> ConnectToDiscordAsync();
        Task<bool> UpdateBotStatusAsync();
    }
}
