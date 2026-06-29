using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiWorker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreLocations",
                columns: table => new
                {
                    StoreLocationId = table.Column<int>(type: "integer", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreLocations", x => x.StoreLocationId);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO "StoreLocations" ("StoreLocationId", "DisplayName", "Address")
                SELECT DISTINCT ON ("StoreLocationId") "StoreLocationId", "StoreDisplayName", "StoreAddress"
                FROM "StoreProducts"
                ORDER BY "StoreLocationId";
                """);

            migrationBuilder.DropColumn(
                name: "StoreAddress",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "StoreDisplayName",
                table: "StoreProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreAddress",
                table: "StoreProducts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoreDisplayName",
                table: "StoreProducts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE "StoreProducts" sp
                SET "StoreDisplayName" = sl."DisplayName",
                    "StoreAddress" = sl."Address"
                FROM "StoreLocations" sl
                WHERE sp."StoreLocationId" = sl."StoreLocationId";
                """);

            migrationBuilder.DropTable(
                name: "StoreLocations");
        }
    }
}
