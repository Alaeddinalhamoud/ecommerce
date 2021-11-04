using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class FreeshippingTabable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "freeShipping",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "taxable",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "freeShipping",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "taxable",
                table: "Products");
        }
    }
}
