using DSharpPlus.Entities;
using LundBot.Config;
using LundBot.Enums;
using LundBot.Interfaces.Services;
using LundBot.Interfaces.Services.Discord;
using Microsoft.Extensions.Options;

namespace LundBot.Services
{
    public sealed class UserService : IUserService
    {
        private readonly IDiscordGuildService _discordGuildService;
        private readonly IDiscordMemberService _discordMemberService;
        private readonly DiscordConfig _discordConfig;

        public UserService(
            IDiscordGuildService discordGuildService,
            IDiscordMemberService discordMemberService,
            IOptions<DiscordConfig> discordConfig
        )
        {
            _discordGuildService = discordGuildService;
            _discordMemberService = discordMemberService;
            _discordConfig = discordConfig.Value;
        }

        public async Task<bool> IsUserAdminAsync(ulong userId, ulong guildId)
        {
            DiscordGuild guild = await _discordGuildService.GetGuildAsync(guildId);

            var member = await _discordMemberService.GetMemberAsync(guild, userId);

            if (
                await _discordMemberService.MemberHasPermission(
                    member,
                    DiscordPermission.Administrator
                )
            )
            {
                return true;
            }

            return await _discordMemberService.IsMemberAdminInGuildAsync(member, guild);
        }

        public async Task<bool> IsUserOwnerAsync(ulong userId, ulong guildId)
        {
            DiscordGuild guild = await _discordGuildService.GetGuildAsync(guildId);

            if (guild.OwnerId == userId)
            {
                return true;
            }

            var member = await _discordMemberService.GetMemberAsync(guild, userId);
            var ownerRole = await _discordGuildService.GetRoleByIdAsync(
                guild,
                _discordConfig.Roles[DiscordRoles.Owner.Key]
            );

            return _discordMemberService.MemberHasRole(member, ownerRole);
        }

        public async Task<bool> IsUserABot(ulong userId, ulong guildId)
        {
            DiscordGuild guild = await _discordGuildService.GetGuildAsync(guildId);

            var member = await _discordMemberService.GetMemberAsync(guild, userId);

            if (member.IsBot)
            {
                return true;
            }

            var botRole = await _discordGuildService.GetRoleByIdAsync(
                guild,
                _discordConfig.Roles[DiscordRoles.Bot.Key]
            );
            return _discordMemberService.MemberHasRole(member, botRole);
        }
    }
}
