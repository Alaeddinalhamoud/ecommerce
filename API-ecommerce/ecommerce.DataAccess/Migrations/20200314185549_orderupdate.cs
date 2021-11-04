using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class orderupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "productId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "vendorId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "cartId",
                table: "Orders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cartId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "productId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "vendorId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
