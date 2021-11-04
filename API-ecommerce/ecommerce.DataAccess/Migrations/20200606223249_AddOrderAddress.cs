using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class AddOrderAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderAddresses",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<string>(nullable: true),
                    houseNumber = table.Column<string>(nullable: true),
                    addressline1 = table.Column<string>(nullable: true),
                    addressline2 = table.Column<string>(nullable: true),
                    street = table.Column<string>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    code = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    createdBy = table.Column<string>(nullable: true),
                    longitude = table.Column<double>(nullable: false),
                    latitude = table.Column<double>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAddresses", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderAddresses");
        }
    }
}
