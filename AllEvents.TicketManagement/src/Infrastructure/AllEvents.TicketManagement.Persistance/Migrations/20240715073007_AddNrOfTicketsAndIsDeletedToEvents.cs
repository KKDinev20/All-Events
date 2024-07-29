using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllEvents.TicketManagement.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddNrOfTicketsAndIsDeletedToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NrOfTickets",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NrOfTickets",
                table: "Events");
        }
    }
}
