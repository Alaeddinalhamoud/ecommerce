using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class Alertfunctinallity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "buttonCaption",
                table: "Sliders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isEnabled",
                table: "Sliders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "salesEmail",
                table: "Settings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "packageType",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(nullable: true),
                    body = table.Column<string>(nullable: true),
                    alertType = table.Column<int>(nullable: false),
                    isEnabled = table.Column<bool>(nullable: false),
                    createdBy = table.Column<string>(nullable: true),
                    modifiedBy = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropColumn(
                name: "buttonCaption",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "isEnabled",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "salesEmail",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "packageType",
                table: "Products");
        }
    }
}
