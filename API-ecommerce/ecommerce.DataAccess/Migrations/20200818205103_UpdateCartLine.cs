using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class UpdateCartLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "freeShipping",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "totalFreeTax",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "totalTax",
                table: "Carts");

            migrationBuilder.AddColumn<double>(
                name: "taxCost",
                table: "Orders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "freeShipping",
                table: "CartLines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "freeTax",
                table: "CartLines",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "taxCost",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "freeShipping",
                table: "CartLines");

            migrationBuilder.DropColumn(
                name: "freeTax",
                table: "CartLines");

            migrationBuilder.AddColumn<bool>(
                name: "freeShipping",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "totalFreeTax",
                table: "Carts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "totalTax",
                table: "Carts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
