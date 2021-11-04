using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class createdy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userId",
                table: "WishLists");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "ProductSpecification");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ProductSpecification",
                newName: "value");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "WishLists",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "Reviews",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "ProductType",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "ProductSpecification",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "Products",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "Orders",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "Medias",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "Categories",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "Carts",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "Brands",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "Addresses",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "WishLists");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "ProductSpecification");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "ProductSpecification",
                newName: "Value");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "WishLists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "ProductType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "ProductSpecification",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Medias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
