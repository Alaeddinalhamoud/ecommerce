using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.Auth.Migrations
{
    public partial class updatedatecreatedate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "createDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "modifiedBy",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updateDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "modifiedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "updateDate",
                table: "AspNetUsers");
        }
    }
}
