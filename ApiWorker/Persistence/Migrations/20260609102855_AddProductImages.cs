using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiWorker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryJson",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "ManufacturerJson",
                table: "StoreProducts");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "StoreProducts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerName",
                table: "StoreProducts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ProductImageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    S3Key = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.ProductImageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "ManufacturerName",
                table: "StoreProducts");

            migrationBuilder.AddColumn<string>(
                name: "CategoryJson",
                table: "StoreProducts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "ImageUrls",
                table: "StoreProducts",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ManufacturerJson",
                table: "StoreProducts",
                type: "text",
                nullable: true);
        }
    }
}
