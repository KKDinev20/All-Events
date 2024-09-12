using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllEvents.TicketManagement.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldInExternalUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountSpent",
                table: "ExternalUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmountSpent",
                table: "ExternalUsers");
        }
    }
}
