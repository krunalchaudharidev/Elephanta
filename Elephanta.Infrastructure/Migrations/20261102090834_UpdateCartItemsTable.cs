using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elephanta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartItemsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Users_UserId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "CartItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "CartItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Users_UserId",
                table: "CartItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
