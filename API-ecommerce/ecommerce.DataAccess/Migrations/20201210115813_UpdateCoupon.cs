using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class UpdateCoupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfUse",
                table: "Coupons",
                newName: "numberOfUse");

            migrationBuilder.AddColumn<int>(
                name: "maximumDiscount",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "maximumSpend",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "maximumDiscount",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "maximumSpend",
                table: "Coupons");

            migrationBuilder.RenameColumn(
                name: "numberOfUse",
                table: "Coupons",
                newName: "NumberOfUse");
        }
    }
}
