namespace LundBot.Application.Discord.Roles
{
    public sealed record DiscordRoleDto
    {
        public ulong Id { get; }
        public string Name { get; }

        public DiscordRoleDto(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
