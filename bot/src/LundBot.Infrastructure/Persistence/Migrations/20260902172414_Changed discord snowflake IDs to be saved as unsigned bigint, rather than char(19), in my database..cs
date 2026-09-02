using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeddiscordsnowflakeIDstobesavedasunsignedbigintratherthanchar19inmydatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaderboardScoreSources");

            migrationBuilder.DropTable(
                name: "WelcomeMessages");

            migrationBuilder.RenameColumn(
                name: "WebsiteTrafficMessagesId",
                table: "WebsiteTrafficMessages",
                newName: "WebsiteTrafficMessageId");

            migrationBuilder.RenameIndex(
                name: "WebsiteTrafficMessages_index_1",
                table: "WebsiteTrafficMessages",
                newName: "WebsiteTrafficMessage_index_1");

            migrationBuilder.RenameColumn(
                name: "LeaderboardScoresId",
                table: "LeaderboardScores",
                newName: "LeaderboardScoreId");

            migrationBuilder.RenameColumn(
                name: "LeaderboardsId",
                table: "LeaderboardScores",
                newName: "LeaderboardId");

            migrationBuilder.RenameIndex(
                name: "LeaderboardScores_index_2",
                table: "LeaderboardScores",
                newName: "LeaderboardScore_index_2");

            migrationBuilder.RenameIndex(
                name: "IX_LeaderboardScores_LeaderboardsId",
                table: "LeaderboardScores",
                newName: "IX_LeaderboardScore_LeaderboardsId");

            migrationBuilder.RenameColumn(
                name: "LeaderboardsId",
                table: "LeaderboardMessages",
                newName: "LeaderboardId");

            migrationBuilder.AlterColumn<ulong>(
                name: "DiscordMessageId",
                table: "WebsiteTrafficMessages",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(19)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "DiscordUserId",
                table: "LeaderboardScores",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(19)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "DiscordServerId",
                table: "Leaderboards",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(19)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "DiscordChannelId",
                table: "Leaderboards",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(19)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "DiscordMessageId",
                table: "LeaderboardMessages",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(19)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaderboardScoreSource");

            migrationBuilder.DropTable(
                name: "MemberJoinMessages");

            migrationBuilder.RenameColumn(
                name: "WebsiteTrafficMessageId",
                table: "WebsiteTrafficMessages",
                newName: "WebsiteTrafficMessagesId");

            migrationBuilder.RenameIndex(
                name: "WebsiteTrafficMessage_index_1",
                table: "WebsiteTrafficMessages",
                newName: "WebsiteTrafficMessages_index_1");

            migrationBuilder.RenameColumn(
                name: "LeaderboardScoreId",
                table: "LeaderboardScores",
                newName: "LeaderboardScoresId");

            migrationBuilder.RenameColumn(
                name: "LeaderboardId",
                table: "LeaderboardScores",
                newName: "LeaderboardsId");

            migrationBuilder.RenameIndex(
                name: "LeaderboardScore_index_2",
                table: "LeaderboardScores",
                newName: "LeaderboardScores_index_2");

            migrationBuilder.RenameIndex(
                name: "IX_LeaderboardScore_LeaderboardsId",
                table: "LeaderboardScores",
                newName: "IX_LeaderboardScores_LeaderboardsId");

            migrationBuilder.RenameColumn(
                name: "LeaderboardId",
                table: "LeaderboardMessages",
                newName: "LeaderboardsId");

            migrationBuilder.AlterColumn<string>(
                name: "DiscordMessageId",
                table: "WebsiteTrafficMessages",
                type: "char(19)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DiscordUserId",
                table: "LeaderboardScores",
                type: "char(19)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DiscordServerId",
                table: "Leaderboards",
                type: "char(19)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DiscordChannelId",
                table: "Leaderboards",
                type: "char(19)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DiscordMessageId",
                table: "LeaderboardMessages",
                type: "char(19)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LeaderboardScoreSources",
                columns: table => new
                {
                    LeaderboardScoreSourceId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeaderboardsId = table.Column<uint>(type: "int unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)"),
                    DiscordUserIdActor = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiscordUserIdTarget = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderboardScoreSources", x => x.LeaderboardScoreSourceId);
                    table.ForeignKey(
                        name: "fk_leaderboard_score_sources_leaderboards",
                        column: x => x.LeaderboardsId,
                        principalTable: "Leaderboards",
                        principalColumn: "LeaderboardsId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WelcomeMessages",
                columns: table => new
                {
                    WelcomeMessagesId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)"),
                    DiscordMessageId = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DiscordUserId = table.Column<string>(type: "char(19)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WelcomeMessages", x => x.WelcomeMessagesId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderboardScoreSources_LeaderboardsId",
                table: "LeaderboardScoreSources",
                column: "LeaderboardsId");

            migrationBuilder.CreateIndex(
                name: "LeaderboardScoreSources_index_2",
                table: "LeaderboardScoreSources",
                columns: new[] { "LeaderboardsId", "DiscordUserIdActor", "DiscordUserIdTarget" });

            migrationBuilder.CreateIndex(
                name: "WelcomeMessages_index_1",
                table: "WelcomeMessages",
                column: "DiscordUserId",
                unique: true);
        }
    }
}
