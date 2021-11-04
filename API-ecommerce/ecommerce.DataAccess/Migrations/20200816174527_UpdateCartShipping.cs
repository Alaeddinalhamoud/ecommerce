using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class UpdateCartShipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total",
                table: "Carts");

            migrationBuilder.AddColumn<bool>(
                name: "freeShipping",
                table: "Carts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "totalFreeTax",
                table: "Carts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "totalTax",
                table: "Carts",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "total",
                table: "Carts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
