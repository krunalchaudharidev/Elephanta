using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elephanta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleteColumnsInProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Medias_MediaId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Medias_MediaId",
                table: "ProductImages");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Products",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Medias_MediaId",
                table: "Categories",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Medias_MediaId",
                table: "ProductImages",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Medias_MediaId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Medias_MediaId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Medias_MediaId",
                table: "Categories",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Medias_MediaId",
                table: "ProductImages",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id");
        }
    }
}
