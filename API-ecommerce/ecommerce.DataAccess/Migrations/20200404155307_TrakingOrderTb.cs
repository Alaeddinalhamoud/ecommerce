using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class TrakingOrderTb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackingOrders",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(nullable: true),
                    trackingStatus = table.Column<int>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    userId = table.Column<string>(nullable: true),
                    updateDate = table.Column<DateTime>(nullable: false),
                    courierTrackingNumber = table.Column<string>(nullable: true),
                    curierName = table.Column<string>(nullable: true),
                    trackingUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingOrders", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackingOrders");
        }
    }
}
