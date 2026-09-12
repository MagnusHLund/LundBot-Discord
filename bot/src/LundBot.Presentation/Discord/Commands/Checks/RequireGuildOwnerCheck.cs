using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using LundBot.Application.Discord.Roles;

namespace LundBot.Presentation.Discord.Commands.Checks
{
    public sealed class RequireGuildOwnerCheck : IContextCheck<RequireGuildOwnerAttribute>
    {
        private readonly IDiscordRoleService _roleService;

        public RequireGuildOwnerCheck(IDiscordRoleService roleService)
        {
            _roleService = roleService;
        }

        public async ValueTask<string?> ExecuteCheckAsync(RequireGuildOwnerAttribute attribute, CommandContext context)
        {
            if (context.Guild is null)
            {
                return "This command can only be used in a server.";
            }

            bool isOwner = await _roleService.IsMemberOwnerAsync(context.User.Id, context.Guild.Id);

            if (!isOwner)
            {
                return "You must be the server owner to use this command.";
            }

            return null;
        }
    }
}
