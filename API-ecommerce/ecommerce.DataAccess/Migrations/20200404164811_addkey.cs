using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class addkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "curierName",
                table: "TrackingOrders");

            migrationBuilder.AddColumn<string>(
                name: "curierCopmany",
                table: "TrackingOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "curierCopmany",
                table: "TrackingOrders");

            migrationBuilder.AddColumn<string>(
                name: "curierName",
                table: "TrackingOrders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
