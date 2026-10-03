using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ScopeMemberJoinMessagesByGuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing rows have no guild or channel identity and cannot be backfilled reliably.
            migrationBuilder.Sql("DELETE FROM `MemberJoinMessages`;");

            migrationBuilder.DropIndex(
                name: "MemberJoinMessages_index_1",
                table: "MemberJoinMessages");

            migrationBuilder.AddColumn<ulong>(
                name: "DiscordChannelId",
                table: "MemberJoinMessages",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "GuildId",
                table: "MemberJoinMessages",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "MemberJoinMessages_index_1",
                table: "MemberJoinMessages",
                columns: new[] { "GuildId", "DiscordUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rows from multiple guilds can share a user ID, which would violate the restored user-only unique index.
            migrationBuilder.Sql("DELETE FROM `MemberJoinMessages`;");

            migrationBuilder.DropIndex(
                name: "MemberJoinMessages_index_1",
                table: "MemberJoinMessages");

            migrationBuilder.DropColumn(
                name: "DiscordChannelId",
                table: "MemberJoinMessages");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "MemberJoinMessages");

            migrationBuilder.CreateIndex(
                name: "MemberJoinMessages_index_1",
                table: "MemberJoinMessages",
                column: "DiscordUserId",
                unique: true);
        }
    }
}
