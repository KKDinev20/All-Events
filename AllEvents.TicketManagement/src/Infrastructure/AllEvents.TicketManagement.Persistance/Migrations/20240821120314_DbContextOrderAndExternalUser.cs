using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllEvents.TicketManagement.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DbContextOrderAndExternalUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ExternalUsers_Id",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EventId",
                table: "Orders",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalUsers_Email",
                table: "ExternalUsers",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Events_EventId",
                table: "Orders",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ExternalUsers_ExternalUserId",
                table: "Orders",
                column: "ExternalUserId",
                principalTable: "ExternalUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Events_EventId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ExternalUsers_ExternalUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EventId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_ExternalUsers_Email",
                table: "ExternalUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ExternalUsers_Id",
                table: "Orders",
                column: "Id",
                principalTable: "ExternalUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
