using LundBot.Application.Common.Discord;

namespace LundBot.Infrastructure.Discord.Services
{
    public class DiscordRoleService : IDiscordRoleService
    {
        public Task<bool> DoesMemberHaveRoleAsync(ulong memberId, ulong guildId, ulong roleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsMemberAdministratorAsync(ulong memberId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsMemberOwnerAsync(ulong memberId, ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
