using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Application.Migrations
{
    /// <inheritdoc />
    public partial class Add_Store_Location_Id_To_Order_Product_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreLocationId",
                table: "OrderProducts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreLocationId",
                table: "OrderProducts");
        }
    }
}
