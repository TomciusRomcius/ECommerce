using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiWorker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeStoreProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "StoreProducts");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_ProductId",
                table: "StoreProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreProducts_Products_ProductId",
                table: "StoreProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreProducts_Products_ProductId",
                table: "StoreProducts");

            migrationBuilder.DropIndex(
                name: "IX_StoreProducts_ProductId",
                table: "StoreProducts");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "StoreProducts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "StoreProducts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StoreProducts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ManufacturerId",
                table: "StoreProducts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "StoreProducts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "StoreProducts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "StoreProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                UPDATE "StoreProducts" AS store_product
                SET
                    "Name" = product."Name",
                    "Description" = product."Description",
                    "Price" = product."Price",
                    "ManufacturerId" = product."ManufacturerId",
                    "CategoryId" = product."CategoryId",
                    "ManufacturerName" = manufacturer."Name",
                    "CategoryName" = category."Name"
                FROM "Products" AS product
                INNER JOIN "Manufacturers" AS manufacturer ON manufacturer."ManufacturerId" = product."ManufacturerId"
                INNER JOIN "Categories" AS category ON category."CategoryId" = product."CategoryId"
                WHERE store_product."ProductId" = product."ProductId";
                """);
        }
    }
}
