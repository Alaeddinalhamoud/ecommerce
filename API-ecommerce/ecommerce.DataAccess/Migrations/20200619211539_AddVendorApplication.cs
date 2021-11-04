using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class AddVendorApplication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorApplications",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fullName = table.Column<string>(nullable: true),
                    companyName = table.Column<string>(nullable: true),
                    companyVAT = table.Column<string>(nullable: true),
                    workEmail = table.Column<string>(nullable: true),
                    workTel = table.Column<string>(nullable: true),
                    companyAddress = table.Column<string>(nullable: true),
                    approved = table.Column<bool>(nullable: false),
                    createdBy = table.Column<string>(nullable: true),
                    modifiedBy = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorApplications", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorApplications");
        }
    }
}
