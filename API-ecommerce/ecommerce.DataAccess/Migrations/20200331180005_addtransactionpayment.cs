using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class addtransactionpayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    userId = table.Column<string>(nullable: true),
                    fullName = table.Column<string>(nullable: true),
                    orderId = table.Column<string>(nullable: true),
                    amount = table.Column<double>(nullable: false),
                    status = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransactions");
        }
    }
}
