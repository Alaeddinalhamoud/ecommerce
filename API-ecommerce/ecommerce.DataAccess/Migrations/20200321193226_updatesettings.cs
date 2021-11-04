using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class updatesettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "Settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "Settings");
        }
    }
}
