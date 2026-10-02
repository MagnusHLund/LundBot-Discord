using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesThatTriggerKicksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RolesThatTriggerKicks",
                columns: table => new
                {
                    AutoKickRoleId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    GuildId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Reason = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(3)", nullable: false, defaultValueSql: "UTC_TIMESTAMP(3)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesThatTriggerKicks", x => x.AutoKickRoleId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "RolesThatTriggerKicks_index_1",
                table: "RolesThatTriggerKicks",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "RolesThatTriggerKicks_index_2",
                table: "RolesThatTriggerKicks",
                column: "RoleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolesThatTriggerKicks");
        }
    }
}
