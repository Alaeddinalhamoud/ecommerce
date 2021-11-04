using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class FreeTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "taxable",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "freeTax",
                table: "Products",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "freeTax",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "taxable",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
