using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiWorker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductManufacturerCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    ManufacturerId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.ManufacturerId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ManufacturerId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ManufacturerId",
                table: "Products",
                column: "ManufacturerId");

            migrationBuilder.Sql(
                """
                INSERT INTO "Manufacturers" ("ManufacturerId", "Name")
                SELECT DISTINCT ON ("ManufacturerId") "ManufacturerId", "ManufacturerName"
                FROM "StoreProducts"
                ORDER BY "ManufacturerId"
                ON CONFLICT ("ManufacturerId") DO UPDATE SET "Name" = EXCLUDED."Name";

                INSERT INTO "Categories" ("CategoryId", "Name")
                SELECT DISTINCT ON ("CategoryId") "CategoryId", "CategoryName"
                FROM "StoreProducts"
                ORDER BY "CategoryId"
                ON CONFLICT ("CategoryId") DO UPDATE SET "Name" = EXCLUDED."Name";

                INSERT INTO "Products" ("ProductId", "Name", "Description", "Price", "ManufacturerId", "CategoryId")
                SELECT DISTINCT ON ("ProductId") "ProductId", "Name", "Description", "Price", "ManufacturerId", "CategoryId"
                FROM "StoreProducts"
                ORDER BY "ProductId"
                ON CONFLICT ("ProductId") DO UPDATE SET
                    "Name" = EXCLUDED."Name",
                    "Description" = EXCLUDED."Description",
                    "Price" = EXCLUDED."Price",
                    "ManufacturerId" = EXCLUDED."ManufacturerId",
                    "CategoryId" = EXCLUDED."CategoryId";
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Products_ProductId",
                table: "ProductImages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
