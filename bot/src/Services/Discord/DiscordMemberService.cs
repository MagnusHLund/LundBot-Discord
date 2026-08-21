using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Interfaces.Services.Discord;
using Serilog;

namespace LundBot.Services.Discord
{
    public sealed class DiscordMemberService : IDiscordMemberService
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<DiscordMemberService>();

        public async Task<DiscordMember> GetMemberAsync(DiscordGuild guild, ulong userId)
        {
            _logger.Information(
                "Fetching member with ID {UserId} from guild {GuildId}...",
                userId,
                guild.Id
            );

            try
            {
                return await guild.GetMemberAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to fetch member with ID {UserId} from guild {GuildId}.",
                    userId,
                    guild.Id
                );

                throw;
            }
        }

        public bool MemberHasRole(DiscordMember member, DiscordRole role)
        {
            _logger.Information(
                "Checking if user with ID {MemberId} has role with ID {RoleId}...",
                member.Id,
                role.Id
            );

            try
            {
                return member.Roles.Any(r => r.Id == role.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check if user with ID {MemberId} has role with ID {RoleId}.",
                    member.Id,
                    role.Id
                );
                throw;
            }
        }

        public async Task KickMemberAsync(DiscordMember member, string reason)
        {
            _logger.Information(
                "Kicking member with ID {MemberId} for reason: {Reason}...",
                member.Id,
                reason
            );

            try
            {
                await member.RemoveAsync(reason);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to kick member with ID {MemberId} for reason: {Reason}.",
                    member.Id,
                    reason
                );
                throw;
            }
        }

        public async Task<bool> MemberHasPermission(DiscordMember member, Permissions permission)
        {
            _logger.Information(
                "Checking if member with ID {MemberId} has permission {Permission}...",
                member.Id,
                permission
            );

            try
            {
                return member.Permissions.HasFlag(permission);
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check if member with ID {MemberId} has permission {Permission}.",
                    member.Id,
                    permission
                );
                throw;
            }
        }

        public async Task<bool> IsMemberAdminInGuildAsync(DiscordMember member, DiscordGuild guild)
        {
            _logger.Information(
                "Checking if member with ID {MemberId} is an admin in guild {GuildId}...",
                member.Id,
                guild.Id
            );

            try
            {
                var adminRoles = guild
                    .Roles.Values.Where(r => r.Permissions.HasFlag(Permissions.Administrator))
                    .ToList();

                if (!adminRoles.Any())
                {
                    _logger.Warning(
                        "No admin role found in guild {GuildId}. Cannot determine if member {MemberId} is an admin.",
                        guild.Id,
                        member.Id
                    );
                    return false;
                }

                if (member.Roles.Any(r => adminRoles.Contains(r)))
                {
                    _logger.Information(
                        "Member with ID {MemberId} is an admin in guild {GuildId}.",
                        member.Id,
                        guild.Id
                    );
                    return true;
                }
                else
                {
                    _logger.Information(
                        "Member with ID {MemberId} is NOT an admin in guild {GuildId}.",
                        member.Id,
                        guild.Id
                    );
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check if member with ID {MemberId} is an admin in guild {GuildId}.",
                    member.Id,
                    guild.Id
                );
                throw;
            }
        }

        public async Task<bool> IsMemberOwnerInGuildAsync(DiscordMember member, DiscordGuild guild)
        {
            _logger.Information(
                "Checking if member with ID {MemberId} is the owner of guild {GuildId}...",
                member.Id,
                guild.Id
            );

            try
            {
                return guild.OwnerId == member.Id;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check if member with ID {MemberId} is the owner of guild {GuildId}.",
                    member.Id,
                    guild.Id
                );
                throw;
            }
        }
    }
}
