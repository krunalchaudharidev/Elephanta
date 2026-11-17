using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elephanta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOfferTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsageLimitPerCustomer",
                table: "Offers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsageLimitPerCustomer",
                table: "Offers");
        }
    }
}
