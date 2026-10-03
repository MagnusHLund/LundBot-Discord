using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedWebsiteTrafficChanneldatabasetable : Migration
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
                    DiscordServerId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DiscordChannelId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Title = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LeaderboardType = table.Column<string>(type: "varchar(32)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboards", x => x.LeaderboardsId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MemberJoinMessages",
                columns: table => new
                {
                    MemberJoinMessageId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DiscordUserId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)"),
                    DiscordMessageId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberJoinMessages", x => x.MemberJoinMessageId);
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
                name: "WebsiteTrafficAnalyticsChannels",
                columns: table => new
                {
                    WebsiteTrafficAnalyticsChannelId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteTrafficAnalyticsChannels", x => x.WebsiteTrafficAnalyticsChannelId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaderboardMessages",
                columns: table => new
                {
                    LeaderboardMessagesId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardId = table.Column<uint>(type: "int unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)"),
                    DiscordMessageId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardMessages", x => x.LeaderboardMessagesId);
                    table.ForeignKey(
                        name: "fk_leaderboard_messages_leaderboards",
                        column: x => x.LeaderboardId,
                        principalTable: "Leaderboards",
                        principalColumn: "LeaderboardsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaderboardScores",
                columns: table => new
                {
                    LeaderboardScoreId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardId = table.Column<uint>(type: "int unsigned", nullable: false),
                    DiscordUserId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Score = table.Column<uint>(type: "int unsigned", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardScores", x => x.LeaderboardScoreId);
                    table.ForeignKey(
                        name: "fk_leaderboard_scores_leaderboards",
                        column: x => x.LeaderboardId,
                        principalTable: "Leaderboards",
                        principalColumn: "LeaderboardsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaderboardScoreSource",
                columns: table => new
                {
                    LeaderboardScoreSourceId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardId = table.Column<uint>(type: "int unsigned", nullable: false),
                    DiscordUserIdActor = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    DiscordUserIdTarget = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardScoreSource", x => x.LeaderboardScoreSourceId);
                    table.ForeignKey(
                        name: "fk_leaderboard_score_source_leaderboards",
                        column: x => x.LeaderboardId,
                        principalTable: "Leaderboards",
                        principalColumn: "LeaderboardsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WebsiteTrafficMessages",
                columns: table => new
                {
                    WebsiteTrafficMessageId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WebsiteTrafficAnalyticsChannelId = table.Column<uint>(type: "int unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)"),
                    DiscordMessageId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteTrafficMessages", x => x.WebsiteTrafficMessageId);
                    table.ForeignKey(
                        name: "fk_website_traffic_messages_channels",
                        column: x => x.WebsiteTrafficAnalyticsChannelId,
                        principalTable: "WebsiteTrafficAnalyticsChannels",
                        principalColumn: "WebsiteTrafficAnalyticsChannelId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardMessages_LeaderboardsId",
                table: "LeaderboardMessages",
                column: "LeaderboardId");

            migrationBuilder.CreateIndex(
                name: "LeaderboardMessages_index_2",
                table: "LeaderboardMessages",
                columns: new[] { "LeaderboardId", "DiscordMessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_LeaderboardType",
                table: "Leaderboards",
                column: "LeaderboardType");

            migrationBuilder.CreateIndex(
                name: "leaderboards_index_2",
                table: "Leaderboards",
                columns: new[] { "DiscordServerId", "DiscordChannelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "leaderboards_index_3",
                table: "Leaderboards",
                columns: new[] { "DiscordServerId", "LeaderboardType" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardScore_LeaderboardsId",
                table: "LeaderboardScores",
                column: "LeaderboardId");

            migrationBuilder.CreateIndex(
                name: "LeaderboardScore_index_2",
                table: "LeaderboardScores",
                columns: new[] { "DiscordUserId", "LeaderboardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardScoreSource_LeaderboardsId",
                table: "LeaderboardScoreSource",
                column: "LeaderboardId");

            migrationBuilder.CreateIndex(
                name: "LeaderboardScoreSource_index_2",
                table: "LeaderboardScoreSource",
                columns: new[] { "LeaderboardId", "DiscordUserIdActor", "DiscordUserIdTarget" });

            migrationBuilder.CreateIndex(
                name: "MemberJoinMessages_index_1",
                table: "MemberJoinMessages",
                column: "DiscordUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UniqueIp",
                table: "WebsiteTraffic",
                column: "HashedIp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WebsiteTrafficAnalyticsChannels_index_1",
                table: "WebsiteTrafficAnalyticsChannels",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "WebsiteTrafficAnalyticsChannels_index_2",
                table: "WebsiteTrafficAnalyticsChannels",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteTrafficMessages_WebsiteTrafficAnalyticsChannelId",
                table: "WebsiteTrafficMessages",
                column: "WebsiteTrafficAnalyticsChannelId");

            migrationBuilder.CreateIndex(
                name: "WebsiteTrafficMessage_index_1",
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
                name: "LeaderboardScoreSource");

            migrationBuilder.DropTable(
                name: "MemberJoinMessages");

            migrationBuilder.DropTable(
                name: "WebsiteTraffic");

            migrationBuilder.DropTable(
                name: "WebsiteTrafficMessages");

            migrationBuilder.DropTable(
                name: "Leaderboards");

            migrationBuilder.DropTable(
                name: "WebsiteTrafficAnalyticsChannels");
        }
    }
}
