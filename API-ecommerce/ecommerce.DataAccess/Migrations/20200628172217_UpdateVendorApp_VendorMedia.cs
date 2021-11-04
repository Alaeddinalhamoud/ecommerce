using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class UpdateVendorApp_VendorMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "workTel",
                table: "VendorApplications");

            migrationBuilder.AddColumn<string>(
                name: "account",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bankAddress",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bankName",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "crNumber",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "iBAN",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ownerIdNumber",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "swiftCode",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tel1",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tel2",
                table: "VendorApplications",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VendorMedias",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: true),
                    mediaType = table.Column<int>(nullable: false),
                    alt = table.Column<string>(nullable: true),
                    path = table.Column<string>(nullable: true),
                    vendortId = table.Column<int>(nullable: false),
                    createdBy = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false),
                    modifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorMedias", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorMedias");

            migrationBuilder.DropColumn(
                name: "account",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "bankAddress",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "bankName",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "crNumber",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "iBAN",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "note",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "ownerIdNumber",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "swiftCode",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "tel1",
                table: "VendorApplications");

            migrationBuilder.DropColumn(
                name: "tel2",
                table: "VendorApplications");

            migrationBuilder.AddColumn<string>(
                name: "workTel",
                table: "VendorApplications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
