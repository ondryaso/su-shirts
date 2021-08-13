using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SUShirts.Migrations
{
    public partial class ReservationDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HandledOn",
                table: "Reservations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MadeOn",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandledOn",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "MadeOn",
                table: "Reservations");
        }
    }
}
