using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ecommerce.DataAccess.Migrations
{
    public partial class brandsmig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    houseNumber = table.Column<string>(nullable: false),
                    addressline1 = table.Column<string>(nullable: false),
                    addressline2 = table.Column<string>(nullable: true),
                    street = table.Column<string>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    code = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true),
                    userId = table.Column<string>(nullable: false),
                    longitude = table.Column<double>(nullable: false),
                    latitude = table.Column<double>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: false),
                    userId = table.Column<string>(nullable: false),
                    imagePath = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CartLines",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cartId = table.Column<int>(nullable: false),
                    productId = table.Column<int>(nullable: false),
                    qty = table.Column<int>(nullable: false),
                    price = table.Column<double>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartLines", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    total = table.Column<double>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: true),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: false),
                    userId = table.Column<string>(nullable: false),
                    imagePath = table.Column<string>(nullable: true),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OrderLines",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orderId = table.Column<string>(nullable: true),
                    productId = table.Column<int>(nullable: false),
                    qty = table.Column<int>(nullable: false),
                    price = table.Column<double>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLines", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    productId = table.Column<int>(nullable: false),
                    userId = table.Column<string>(nullable: false),
                    vendorId = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    delivery = table.Column<double>(nullable: false),
                    total = table.Column<double>(nullable: false),
                    isPaid = table.Column<bool>(nullable: false),
                    paymentMethod = table.Column<int>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: false),
                    userId = table.Column<string>(nullable: false),
                    vendorId = table.Column<string>(nullable: true),
                    isDeleted = table.Column<bool>(nullable: false),
                    price = table.Column<double>(nullable: false),
                    categoryId = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    tags = table.Column<string>(nullable: false),
                    isApproved = table.Column<bool>(nullable: false),
                    videoUrl = table.Column<string>(nullable: true),
                    rating = table.Column<double>(nullable: false),
                    placeOfOrigin = table.Column<int>(nullable: false),
                    modelNumber = table.Column<string>(nullable: true),
                    expiryDate = table.Column<DateTime>(nullable: false),
                    barcode = table.Column<int>(nullable: false),
                    inventoryCode = table.Column<string>(nullable: true),
                    LotNumber = table.Column<string>(nullable: true),
                    brandId = table.Column<int>(nullable: false),
                    productTypeId = table.Column<int>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecification",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    userId = table.Column<string>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSpecification", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: false),
                    userId = table.Column<string>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(nullable: false),
                    rating = table.Column<int>(nullable: false),
                    productId = table.Column<int>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "WishLists",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(nullable: false),
                    userId = table.Column<string>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishLists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Medias",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: true),
                    mediaType = table.Column<int>(nullable: false),
                    alt = table.Column<string>(nullable: true),
                    path = table.Column<string>(nullable: true),
                    productId = table.Column<int>(nullable: false),
                    userId = table.Column<string>(nullable: false),
                    createDate = table.Column<DateTime>(nullable: false),
                    updateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medias", x => x.id);
                    table.ForeignKey(
                        name: "FK_Medias_Products_productId",
                        column: x => x.productId,
                        principalTable: "Products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medias_productId",
                table: "Medias",
                column: "productId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "CartLines");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Medias");

            migrationBuilder.DropTable(
                name: "OrderLines");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductSpecification");

            migrationBuilder.DropTable(
                name: "ProductType");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "WishLists");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
