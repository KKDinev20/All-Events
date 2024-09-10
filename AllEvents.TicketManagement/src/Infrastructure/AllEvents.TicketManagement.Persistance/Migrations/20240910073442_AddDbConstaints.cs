using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllEvents.TicketManagement.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddDbConstaints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPercent = table.Column<int>(type: "int", nullable: true),
                    FixedDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.CheckConstraint("CK_Coupon_Discount", "(DiscountPercent IS NOT NULL AND FixedDiscountAmount IS NULL) OR (DiscountPercent IS NULL AND FixedDiscountAmount IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Coupons_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_EventId",
                table: "Coupons",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
      
        }
    }
}
