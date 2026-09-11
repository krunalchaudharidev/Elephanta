using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elephanta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMediaIdTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "OfferImages");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Categories");

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "ProductImages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "OfferImages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MediaId",
                table: "Categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_MediaId",
                table: "ProductImages",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferImages_MediaId",
                table: "OfferImages",
                column: "MediaId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_MediaId",
                table: "Categories",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Medias_MediaId",
                table: "Categories",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OfferImages_Medias_MediaId",
                table: "OfferImages",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Medias_MediaId",
                table: "ProductImages",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Medias_MediaId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_OfferImages_Medias_MediaId",
                table: "OfferImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Medias_MediaId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_MediaId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_OfferImages_MediaId",
                table: "OfferImages");

            migrationBuilder.DropIndex(
                name: "IX_Categories_MediaId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "OfferImages");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ProductImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "OfferImages",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Categories",
                type: "text",
                nullable: true);
        }
    }
}
