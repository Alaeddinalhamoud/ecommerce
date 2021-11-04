using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class addFullNameForAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fullName",
                table: "Addresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fullName",
                table: "Addresses");
        }
    }
}
