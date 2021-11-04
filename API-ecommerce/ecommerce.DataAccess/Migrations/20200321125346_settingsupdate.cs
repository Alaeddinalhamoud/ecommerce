using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class settingsupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    websiteName = table.Column<string>(nullable: true),
                    numberOfSliders = table.Column<int>(nullable: false),
                    pageSize = table.Column<int>(nullable: false),
                    email = table.Column<string>(nullable: true),
                    isSlider = table.Column<bool>(nullable: false),
                    isMostViewdProduct = table.Column<bool>(nullable: false),
                    createdBy = table.Column<string>(nullable: true),
                    modifiedBy = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false),
                    logo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
