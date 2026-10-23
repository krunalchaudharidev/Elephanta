using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elephanta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CustomerSupportChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerSupportRequests_Users_UserId",
                table: "CustomerSupportRequests");

            migrationBuilder.DropIndex(
                name: "IX_CustomerSupportRequests_UserId",
                table: "CustomerSupportRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CustomerSupportRequests");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CustomerSupportRequests",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CustomerSupportRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CustomerSupportRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerSupportRequests_UserId",
                table: "CustomerSupportRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerSupportRequests_Users_UserId",
                table: "CustomerSupportRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
