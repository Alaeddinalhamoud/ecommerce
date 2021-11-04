using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class updatetaxshipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "shippingCost",
                table: "Settings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "tax",
                table: "Settings",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "shippingCost",
                table: "Orders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "subTotal",
                table: "Orders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "tax",
                table: "Orders",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shippingCost",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "tax",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "shippingCost",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "subTotal",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "tax",
                table: "Orders");
        }
    }
}
