using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Migrations
{
    /// <inheritdoc />
    public partial class CreatedDatabaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Leaderboards",
                columns: table => new
                {
                    LeaderboardsId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscordServerId = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiscordChannelId = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboards", x => x.LeaderboardsId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WebsiteTraffic",
                columns: table => new
                {
                    WebsiteTrafficId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HashedIp = table.Column<byte[]>(type: "binary(32)", nullable: false),
                    ClickedInviteButton = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteTraffic", x => x.WebsiteTrafficId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WebsiteTrafficMessages",
                columns: table => new
                {
                    WebsiteTrafficMessagesId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscordMessageId = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteTrafficMessages", x => x.WebsiteTrafficMessagesId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaderboardMessages",
                columns: table => new
                {
                    LeaderboardMessagesId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardsId = table.Column<uint>(type: "int unsigned", nullable: false),
                    DiscordMessageId = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardMessages", x => x.LeaderboardMessagesId);
                    table.ForeignKey(
                        name: "fk_leaderboard_messages_leaderboards",
                        column: x => x.LeaderboardsId,
                        principalTable: "Leaderboards",
                        principalColumn: "LeaderboardsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaderboardScores",
                columns: table => new
                {
                    LeaderboardScoresId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardsId = table.Column<uint>(type: "int unsigned", nullable: false),
                    DiscordUserId = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Score = table.Column<uint>(type: "int unsigned", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardScores", x => x.LeaderboardScoresId);
                    table.ForeignKey(
                        name: "fk_leaderboard_scores_leaderboards",
                        column: x => x.LeaderboardsId,
                        principalTable: "Leaderboards",
                        principalColumn: "LeaderboardsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UpvotingLeaderBoard",
                columns: table => new
                {
                    UpvotingLeaderboardId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardsId = table.Column<uint>(type: "int unsigned", nullable: false),
                    DiscordUserIdVoter = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiscordUserIdTarget = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
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
                name: "IX_LeaderboardMessages_LeaderboardsId",
                table: "LeaderboardMessages",
                column: "LeaderboardsId");

            migrationBuilder.CreateIndex(
                name: "LeaderboardMessages_index_2",
                table: "LeaderboardMessages",
                columns: new[] { "LeaderboardsId", "DiscordMessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "leaderboards_index_2",
                table: "Leaderboards",
                columns: new[] { "DiscordServerId", "DiscordChannelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardScores_LeaderboardsId",
                table: "LeaderboardScores",
                column: "LeaderboardsId");

            migrationBuilder.CreateIndex(
                name: "LeaderboardScores_index_2",
                table: "LeaderboardScores",
                columns: new[] { "DiscordUserId", "LeaderboardsId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UpvotingLeaderBoard_LeaderboardsId",
                table: "UpvotingLeaderBoard",
                column: "LeaderboardsId");

            migrationBuilder.CreateIndex(
                name: "UpvotingLeaderBoard_index_2",
                table: "UpvotingLeaderBoard",
                columns: new[] { "LeaderboardsId", "DiscordUserIdVoter", "DiscordUserIdTarget" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UniqueIp",
                table: "WebsiteTraffic",
                column: "HashedIp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WebsiteTrafficMessages_index_1",
                table: "WebsiteTrafficMessages",
                column: "DiscordMessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaderboardMessages");

            migrationBuilder.DropTable(
                name: "LeaderboardScores");

            migrationBuilder.DropTable(
                name: "UpvotingLeaderBoard");

            migrationBuilder.DropTable(
                name: "WebsiteTraffic");

            migrationBuilder.DropTable(
                name: "WebsiteTrafficMessages");

            migrationBuilder.DropTable(
                name: "Leaderboards");
        }
    }
}
