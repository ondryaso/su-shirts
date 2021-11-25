using Microsoft.EntityFrameworkCore.Migrations;

namespace SUShirts.Migrations
{
    public partial class InternalInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "Reservations",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternalNote",
                table: "Reservations",
                type: "TEXT",
                maxLength: 512,
                nullable: true);

            migrationBuilder.Sql("UPDATE Reservations SET Handled = 2 WHERE Handled = 1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "InternalNote",
                table: "Reservations");
            
            migrationBuilder.Sql("UPDATE Reservations SET Handled = 0 WHERE Handled = 1");
            migrationBuilder.Sql("UPDATE Reservations SET Handled = 1 WHERE Handled > 1");
        }
    }
}
