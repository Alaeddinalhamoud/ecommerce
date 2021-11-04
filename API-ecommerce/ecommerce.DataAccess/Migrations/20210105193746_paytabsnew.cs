using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class paytabsnew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isSTCPay",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "secretKey",
                table: "Settings",
                newName: "payTabsServerKey");

            migrationBuilder.RenameColumn(
                name: "sTCPayQR",
                table: "Settings",
                newName: "payTabsMerchantProfileID");

            migrationBuilder.RenameColumn(
                name: "merchantEmail",
                table: "Settings",
                newName: "payTabsClientKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "payTabsServerKey",
                table: "Settings",
                newName: "secretKey");

            migrationBuilder.RenameColumn(
                name: "payTabsMerchantProfileID",
                table: "Settings",
                newName: "sTCPayQR");

            migrationBuilder.RenameColumn(
                name: "payTabsClientKey",
                table: "Settings",
                newName: "merchantEmail");

            migrationBuilder.AddColumn<bool>(
                name: "isSTCPay",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
