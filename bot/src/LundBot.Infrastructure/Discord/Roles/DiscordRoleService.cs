using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Roles;
using Serilog;

namespace LundBot.Infrastructure.Discord.Roles
{
    public class DiscordRoleService : IDiscordRoleService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordRoleService>();

        public DiscordRoleService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task<bool> DoesMemberHaveRoleAsync(ulong memberId, ulong guildId, ulong roleId)
        {
            _logger.Information(
                "Checking if member {MemberId} has role {RoleId} in guild {GuildId}...",
                memberId,
                roleId,
                guildId
            );

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                DiscordMember member = await guild.GetMemberAsync(memberId);

                return member.Roles.Any(role => role.Id == roleId);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check if member {MemberId} has role {RoleId} in guild {GuildId}",
                    memberId,
                    roleId,
                    guildId
                );
                return false;
            }
        }

        public async Task<bool> IsMemberAdministratorAsync(ulong memberId, ulong guildId)
        {
            _logger.Information(
                "Checking if member with ID {MemberId} is an admin in guild {GuildId}...",
                memberId,
                guildId
            );

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                DiscordMember member = await guild.GetMemberAsync(memberId);

                return member.Permissions.HasPermission(DiscordPermission.Administrator);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check if member with ID {MemberId} is an admin in guild {GuildId}",
                    memberId,
                    guildId
                );
                return false;
            }
        }

        public async Task<bool> IsMemberOwnerAsync(ulong memberId, ulong guildId)
        {
            _logger.Information(
                "Checking if member with ID {MemberId} is the owner of guild {GuildId}...",
                memberId,
                guildId
            );

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                DiscordMember member = await guild.GetMemberAsync(memberId);

                if (guild.OwnerId == memberId)
                {
                    return true;
                }

                DiscordRole? role = guild.Roles.Values.FirstOrDefault(x =>
                    string.Equals(x.Name, "owner", StringComparison.OrdinalIgnoreCase)
                );

                if (role is null)
                {
                    return false;
                }

                return await DoesMemberHaveRoleAsync(memberId, guildId, role.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check if member with ID {MemberId} is the owner of guild {GuildId}",
                    memberId,
                    guildId
                );
                return false;
            }
        }
    }
}
