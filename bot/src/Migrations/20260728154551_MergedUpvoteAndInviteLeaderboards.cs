using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Migrations
{
    /// <inheritdoc />
    public partial class MergedUpvoteAndInviteLeaderboards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UpvotingLeaderBoard");

            migrationBuilder.CreateTable(
                name: "LeaderboardScoreEntity",
                columns: table => new
                {
                    LeaderboardScoreSourceId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardsId = table.Column<uint>(type: "int unsigned", nullable: false),
                    DiscordUserIdActor = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiscordUserIdTarget = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardScoreEntity", x => x.LeaderboardScoreSourceId);
                    table.ForeignKey(
                        name: "fk_leaderboard_score_sources_leaderboards",
                        column: x => x.LeaderboardsId,
                        principalTable: "Leaderboards",
                        principalColumn: "LeaderboardsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "leaderboards_index_3",
                table: "Leaderboards",
                columns: new[] { "DiscordServerId", "LeaderboardType" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardScoreSources_LeaderboardsId",
                table: "LeaderboardScoreEntity",
                column: "LeaderboardsId");

            migrationBuilder.CreateIndex(
                name: "LeaderboardScoreSources_index_2",
                table: "LeaderboardScoreEntity",
                columns: new[] { "LeaderboardsId", "DiscordUserIdActor", "DiscordUserIdTarget" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaderboardScoreEntity");

            migrationBuilder.DropIndex(
                name: "leaderboards_index_3",
                table: "Leaderboards");

            migrationBuilder.CreateTable(
                name: "UpvotingLeaderBoard",
                columns: table => new
                {
                    UpvotingLeaderboardId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardsId = table.Column<uint>(type: "int unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)"),
                    DiscordUserIdTarget = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiscordUserIdVoter = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpvotingLeaderBoard", x => x.UpvotingLeaderboardId);
                    table.ForeignKey(
                        name: "fk_upvoting_leaderboard_leaderboards",
                        column: x => x.LeaderboardsId,
                        principalTable: "Leaderboards",
                        principalColumn: "LeaderboardsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UpvotingLeaderBoard_LeaderboardsId",
                table: "UpvotingLeaderBoard",
                column: "LeaderboardsId");

            migrationBuilder.CreateIndex(
                name: "UpvotingLeaderBoard_index_2",
                table: "UpvotingLeaderBoard",
                columns: new[] { "LeaderboardsId", "DiscordUserIdVoter", "DiscordUserIdTarget" },
                unique: true);
        }
    }
}
