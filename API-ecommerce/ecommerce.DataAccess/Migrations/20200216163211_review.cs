using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class review : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Reviews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Reviews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Reviews",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "email",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Reviews");
        }
    }
}
