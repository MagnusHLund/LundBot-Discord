using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LundBot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Movedipuniquekeytobeperguildlevelinsteadofjustlimitingeachwebsitevisittoallguildsfromthesameip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UniqueIp",
                table: "WebsiteTraffic");

            migrationBuilder.AddColumn<uint>(
                name: "WebsiteTrafficAnalyticsChannelId",
                table: "WebsiteTraffic",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteTraffic_WebsiteTrafficAnalyticsChannelId",
                table: "WebsiteTraffic",
                column: "WebsiteTrafficAnalyticsChannelId");

            migrationBuilder.CreateIndex(
                name: "UniqueIp",
                table: "WebsiteTraffic",
                columns: new[] { "HashedIp", "WebsiteTrafficAnalyticsChannelId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_website_traffic_channels",
                table: "WebsiteTraffic",
                column: "WebsiteTrafficAnalyticsChannelId",
                principalTable: "WebsiteTrafficAnalyticsChannels",
                principalColumn: "WebsiteTrafficAnalyticsChannelId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_website_traffic_channels",
                table: "WebsiteTraffic");

            migrationBuilder.DropIndex(
                name: "IX_WebsiteTraffic_WebsiteTrafficAnalyticsChannelId",
                table: "WebsiteTraffic");

            migrationBuilder.DropIndex(
                name: "UniqueIp",
                table: "WebsiteTraffic");

            migrationBuilder.DropColumn(
                name: "WebsiteTrafficAnalyticsChannelId",
                table: "WebsiteTraffic");

            migrationBuilder.CreateIndex(
                name: "UniqueIp",
                table: "WebsiteTraffic",
                column: "HashedIp",
                unique: true);
        }
    }
}
