using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class removeProductfromMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Products_productId",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_productId",
                table: "Medias");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Medias_productId",
                table: "Medias",
                column: "productId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Products_productId",
                table: "Medias",
                column: "productId",
                principalTable: "Products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
