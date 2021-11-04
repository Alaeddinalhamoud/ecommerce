using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class UpdateSettingEnablePaymentMEthods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isCard",
                table: "Settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isCash",
                table: "Settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSTCPay",
                table: "Settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "sTCPayQR",
                table: "Settings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isCard",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "isCash",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "isSTCPay",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "sTCPayQR",
                table: "Settings");
        }
    }
}
