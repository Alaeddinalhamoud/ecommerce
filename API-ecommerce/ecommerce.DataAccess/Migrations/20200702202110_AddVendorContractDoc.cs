using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class AddVendorContractDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isVendorEnabled",
                table: "Settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "vendorAgreementContract",
                table: "Settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isVendorEnabled",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "vendorAgreementContract",
                table: "Settings");
        }
    }
}
