using Microsoft.EntityFrameworkCore.Migrations;

namespace SUShirts.Migrations
{
    public partial class ItemsInStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemsInStock",
                table: "ShirtVariants",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemsInStock",
                table: "ShirtVariants");
        }
    }
}
