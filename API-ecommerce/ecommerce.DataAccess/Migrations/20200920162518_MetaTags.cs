using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class MetaTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetaTags",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    image = table.Column<string>(nullable: true),
                    imageAlt = table.Column<string>(nullable: true),
                    url = table.Column<string>(nullable: true),
                    locale = table.Column<string>(nullable: true),
                    sitename = table.Column<string>(nullable: true),
                    video = table.Column<string>(nullable: true),
                    Keywords = table.Column<string>(nullable: true),
                    createdBy = table.Column<string>(nullable: true),
                    modifiedBy = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaTags", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaTags");
        }
    }
}
