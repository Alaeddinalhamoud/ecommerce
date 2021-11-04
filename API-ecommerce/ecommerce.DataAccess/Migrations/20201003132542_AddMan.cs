using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class AddMan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "enableMaintenance",
                table: "Settings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "enableMaintenance",
                table: "Settings");
        }
    }
}
