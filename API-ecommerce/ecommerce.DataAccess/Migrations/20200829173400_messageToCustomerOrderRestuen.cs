using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class messageToCustomerOrderRestuen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notes",
                table: "OrderReturnCases");

            migrationBuilder.AddColumn<string>(
                name: "actionsToSolve",
                table: "OrderReturnCases",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "OrderReturnCases",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "messageToCustomer",
                table: "OrderReturnCases",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actionsToSolve",
                table: "OrderReturnCases");

            migrationBuilder.DropColumn(
                name: "email",
                table: "OrderReturnCases");

            migrationBuilder.DropColumn(
                name: "messageToCustomer",
                table: "OrderReturnCases");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "OrderReturnCases",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
