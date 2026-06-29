using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Application.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClientSecretAndAddCheckoutUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientSecret",
                table: "PaymentSessions",
                newName: "CheckoutUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckoutUrl",
                table: "PaymentSessions",
                newName: "ClientSecret");
        }
    }
}
