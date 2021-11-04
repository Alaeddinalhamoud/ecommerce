using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class AddedCoupontoCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "typeValue",
                table: "Coupons");

            migrationBuilder.RenameColumn(
                name: "maximumSpend",
                table: "Coupons",
                newName: "value");

            migrationBuilder.AddColumn<int>(
                name: "minimumSpend",
                table: "Coupons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "couponId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "minimumSpend",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "couponId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "Coupons",
                newName: "maximumSpend");

            migrationBuilder.AddColumn<string>(
                name: "typeValue",
                table: "Coupons",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
