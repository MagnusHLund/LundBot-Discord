using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedIncorrectName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaderboardScoreEntity",
                table: "LeaderboardScoreEntity");

            migrationBuilder.RenameTable(
                name: "LeaderboardScoreEntity",
                newName: "LeaderboardScoreSources");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaderboardScoreSources",
                table: "LeaderboardScoreSources",
                column: "LeaderboardScoreSourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LeaderboardScoreSources",
                table: "LeaderboardScoreSources");

            migrationBuilder.RenameTable(
                name: "LeaderboardScoreSources",
                newName: "LeaderboardScoreEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeaderboardScoreEntity",
                table: "LeaderboardScoreEntity",
                column: "LeaderboardScoreSourceId");
        }
    }
}
