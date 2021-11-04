using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class OrderLinesUpdateFreeTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "freeShipping",
                table: "OrderLines",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "freeTax",
                table: "OrderLines",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "freeShipping",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "freeTax",
                table: "OrderLines");
        }
    }
}
