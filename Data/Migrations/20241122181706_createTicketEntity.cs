using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class createTicketEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });


            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "Price", "Type" },
                values: new object[,]
                {
                    { 1, 20.0, "Regular" },
                    { 2, 10.0, "Senior" },
                    { 3, 15.0, "Student" },
                    { 4, 5.0, "Kid" }
                });

            migrationBuilder.AddColumn<int>(
                name: "TicketId",
                table: "SeatReservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_TicketId",
                table: "SeatReservations",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_SeatReservations_Tickets_TicketId",
                table: "SeatReservations",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeatReservations_Tickets_TicketId",
                table: "SeatReservations");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_SeatReservations_TicketId",
                table: "SeatReservations");

            

            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "SeatReservations");

           
        }
    }
}
