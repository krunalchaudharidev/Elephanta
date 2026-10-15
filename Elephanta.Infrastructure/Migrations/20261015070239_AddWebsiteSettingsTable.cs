using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elephanta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebsiteSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SiteName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FacebookURL = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InstagramURL = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhoneNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FreeShippingOnOrdersAbove = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    Address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsMaintenanceMode = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WebsiteSettings",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "FacebookURL", "InstagramURL", "PhoneNo", "SiteName", "UpdatedAt" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), null, new DateTime(2026, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, "My Site", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebsiteSettings");
        }
    }
}
