using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class UpdateSettting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "merchantEmail",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "secretKey",
                table: "Settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "merchantEmail",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "secretKey",
                table: "Settings");
        }
    }
}
