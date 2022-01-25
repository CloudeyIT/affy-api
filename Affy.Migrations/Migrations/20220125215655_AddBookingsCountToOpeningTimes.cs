using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Affy.Migrations.Migrations
{
    public partial class AddBookingsCountToOpeningTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OpeningTimeId",
                table: "Booking",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Booking_OpeningTimeId",
                table: "Booking",
                column: "OpeningTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_OpeningTime_OpeningTimeId",
                table: "Booking",
                column: "OpeningTimeId",
                principalTable: "OpeningTime",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_OpeningTime_OpeningTimeId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_OpeningTimeId",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "OpeningTimeId",
                table: "Booking");
        }
    }
}
