using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedStringEnumLeaderboardType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AlterColumn<string>(
                    name: "LeaderboardType",
                    table: "Leaderboards",
                    type: "varchar(32)",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int"
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboards_LeaderboardType",
                table: "Leaderboards",
                column: "LeaderboardType"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Leaderboards_LeaderboardType", table: "Leaderboards");

            migrationBuilder
                .AlterColumn<int>(
                    name: "LeaderboardType",
                    table: "Leaderboards",
                    type: "int",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "varchar(32)"
                )
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
