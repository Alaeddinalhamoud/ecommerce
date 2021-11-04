using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class MostViewdProductAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "frequency",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "MostViewedProducts",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(nullable: false),
                    frequency = table.Column<int>(nullable: false),
                    lastVisitDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MostViewedProducts", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MostViewedProducts");

            migrationBuilder.AddColumn<int>(
                name: "frequency",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
