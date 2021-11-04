using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class UpdateBankDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vendortId",
                table: "VendorMedias");

            migrationBuilder.AddColumn<string>(
                name: "vendorId",
                table: "VendorMedias",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "vendorId",
                table: "VendorMedias");

            migrationBuilder.AddColumn<int>(
                name: "vendortId",
                table: "VendorMedias",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
