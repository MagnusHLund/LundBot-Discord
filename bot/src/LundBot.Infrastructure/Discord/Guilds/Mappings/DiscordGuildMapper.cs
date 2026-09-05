using DSharpPlus.Entities;
using LundBot.Application.Discord.Guilds;

namespace LundBot.Infrastructure.Discord.Guilds.Mappings
{
    public static class DiscordGuildMapper
    {
        public static DiscordGuildDto Map(DiscordGuild guild)
        {
            return new DiscordGuildDto(guild.Id);
        }
    }
}
