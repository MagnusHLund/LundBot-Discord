using DSharpPlus;
using DSharpPlus.Entities;
using LundBot.Application.Discord.Members;
using LundBot.Application.Discord.Permissions;
using LundBot.Infrastructure.Discord.Members.Mappings;
using LundBot.Infrastructure.Discord.Permissions.Mappings;
using Serilog;

namespace LundBot.Infrastructure.Discord.Members
{
    public sealed class DiscordMemberService : IDiscordMemberService
    {
        private readonly DiscordClient _discordClient;

        private readonly ILogger _logger = Log.ForContext<DiscordMemberService>();

        public DiscordMemberService(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task<DiscordMemberDto?> GetMemberAsync(ulong memberId, ulong guildId)
        {
            _logger.Information("Fetching member with ID {UserId} from guild {GuildId}...", memberId, guildId);

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                DiscordMember member = await guild.GetMemberAsync(memberId);

                return DiscordMemberMapper.Map(member);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to fetch member with ID {UserId} from guild {GuildId}", memberId, guildId);
                return null;
            }
        }

        public async Task<bool> DoesMemberHavePermissionAsync(
            ulong memberId,
            ulong guildId,
            DiscordPermissionEnum permission
        )
        {
            _logger.Information(
                "Checking if member with ID {UserId} has permission {Permission} in guild {GuildId}...",
                memberId,
                permission,
                guildId
            );

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                DiscordMember member = await guild.GetMemberAsync(memberId);

                return member.Permissions.HasFlag(DiscordPermissionMapper.Map(permission));
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Failed to check permission for member with ID {UserId} in guild {GuildId}",
                    memberId,
                    guildId
                );
                return false;
            }
        }

        /// <summary>
        /// Preloads the members of a guild to ensure that the member cache is populated. Prevents API calls to discord, when getting individual members.
        /// </summary>
        /// <param name="guildId">The ID of the guild to preload members for.</param>
        /// <returns>True if the members were successfully preloaded; otherwise, false.</returns>
        public async Task<bool> PreloadMembersAsync(ulong guildId)
        {
            _logger.Information("Preloading members for guild {GuildId}...", guildId);

            try
            {
                DiscordGuild guild = await _discordClient.GetGuildAsync(guildId);
                await guild.RequestMembersAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to preload members for guild {GuildId}.", guildId);
                return false;
            }
        }
    }
}
