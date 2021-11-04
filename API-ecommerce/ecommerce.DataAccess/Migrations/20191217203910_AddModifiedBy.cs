using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class AddModifiedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProductType",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "WishLists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Reviews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "ProductType",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "ProductSpecification",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "barcode",
                table: "Products",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "OrderLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Medias",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Carts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "CartLines",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Brands",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "Addresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "WishLists");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "ProductSpecification");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "CartLines");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ProductType",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "barcode",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
