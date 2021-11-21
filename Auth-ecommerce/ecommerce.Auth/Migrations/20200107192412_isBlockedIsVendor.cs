using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.Auth.Migrations
{
    public partial class isBlockedIsVendor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isBlocked",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isVendor",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBlocked",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "isVendor",
                table: "AspNetUsers");
        }
    }
}
