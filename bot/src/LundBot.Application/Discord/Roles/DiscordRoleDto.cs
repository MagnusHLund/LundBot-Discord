namespace LundBot.Application.Discord.Roles
{
    public sealed record DiscordRoleDto
    {
        public ulong RoleId { get; }
        public string Name { get; }

        public DiscordRoleDto(ulong roleId, string name)
        {
            RoleId = roleId;
            Name = name;
        }
    }
}
