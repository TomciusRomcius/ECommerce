using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiWorker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCartProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartProducts",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    StoreLocationId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartProducts", x => new { x.UserId, x.StoreLocationId, x.ProductId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartProducts_UserId",
                table: "CartProducts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartProducts");
        }
    }
}
