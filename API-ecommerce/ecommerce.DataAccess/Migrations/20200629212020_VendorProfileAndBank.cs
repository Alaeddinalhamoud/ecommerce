using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class VendorProfileAndBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "approved",
                table: "VendorApplications");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "VendorApplications",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VendorBanks",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vendorProfileId = table.Column<int>(nullable: false),
                    bankName = table.Column<string>(nullable: true),
                    bankAddress = table.Column<string>(nullable: true),
                    account = table.Column<string>(nullable: true),
                    swiftCode = table.Column<string>(nullable: true),
                    iBAN = table.Column<string>(nullable: true),
                    createdBy = table.Column<string>(nullable: true),
                    modifiedBy = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorBanks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "VendorProfiles",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vendorId = table.Column<string>(nullable: true),
                    companyName = table.Column<string>(nullable: true),
                    companyVAT = table.Column<string>(nullable: true),
                    workEmail = table.Column<string>(nullable: true),
                    tel1 = table.Column<string>(nullable: true),
                    tel2 = table.Column<string>(nullable: true),
                    crNumber = table.Column<string>(nullable: true),
                    ownerIdNumber = table.Column<string>(nullable: true),
                    companyAddress = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    createdBy = table.Column<string>(nullable: true),
                    modifiedBy = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorProfiles", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorBanks");

            migrationBuilder.DropTable(
                name: "VendorProfiles");

            migrationBuilder.DropColumn(
                name: "status",
                table: "VendorApplications");

            migrationBuilder.AddColumn<bool>(
                name: "approved",
                table: "VendorApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
