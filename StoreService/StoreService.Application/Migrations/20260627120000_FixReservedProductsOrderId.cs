using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreService.Application.Migrations
{
    /// <inheritdoc />
    public partial class FixReservedProductsOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedProducts");

            migrationBuilder.CreateTable(
                name: "ReservedProducts",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreLocationId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedProducts", x => new { x.OrderId, x.StoreLocationId, x.ProductId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedProducts");

            migrationBuilder.CreateTable(
                name: "ReservedProducts",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    StoreLocationId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedProducts", x => new { x.OrderId, x.StoreLocationId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ReservedProducts_StoreLocations_StoreLocationId",
                        column: x => x.StoreLocationId,
                        principalTable: "StoreLocations",
                        principalColumn: "StoreLocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservedProducts_StoreLocationId",
                table: "ReservedProducts",
                column: "StoreLocationId",
                unique: true);
        }
    }
}
