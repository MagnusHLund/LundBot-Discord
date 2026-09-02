using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Removeduniquekeytomaketableworkwiththewarningleaderboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "LeaderboardScoreSources_index_2",
                table: "LeaderboardScoreSources");

            migrationBuilder.CreateIndex(
                name: "LeaderboardScoreSources_index_2",
                table: "LeaderboardScoreSources",
                columns: new[] { "LeaderboardsId", "DiscordUserIdActor", "DiscordUserIdTarget" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "LeaderboardScoreSources_index_2",
                table: "LeaderboardScoreSources");

            migrationBuilder.CreateIndex(
                name: "LeaderboardScoreSources_index_2",
                table: "LeaderboardScoreSources",
                columns: new[] { "LeaderboardsId", "DiscordUserIdActor", "DiscordUserIdTarget" },
                unique: true);
        }
    }
}
