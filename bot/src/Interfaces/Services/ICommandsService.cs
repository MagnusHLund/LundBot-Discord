using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;

namespace LundBot.Interfaces.Services
{
    public interface ICommandsService
    {
        Task RegisterCommandsAsync(DiscordClient discordClient);
        Task LogRegisteredCommandsForGuildsAsync(DiscordClient discordClient);
    }
}
