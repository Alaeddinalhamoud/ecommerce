using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class updateCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_recentlyViewedProducts",
                table: "recentlyViewedProducts");

            migrationBuilder.RenameTable(
                name: "recentlyViewedProducts",
                newName: "RecentlyViewedProducts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updateDate",
                table: "Carts",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "createdBy",
                table: "CartLines",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecentlyViewedProducts",
                table: "RecentlyViewedProducts",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RecentlyViewedProducts",
                table: "RecentlyViewedProducts");

            migrationBuilder.DropColumn(
                name: "createdBy",
                table: "CartLines");

            migrationBuilder.RenameTable(
                name: "RecentlyViewedProducts",
                newName: "recentlyViewedProducts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updateDate",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_recentlyViewedProducts",
                table: "recentlyViewedProducts",
                column: "id");
        }
    }
}
