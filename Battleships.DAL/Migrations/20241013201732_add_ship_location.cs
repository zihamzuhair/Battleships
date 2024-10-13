using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Battleships.DAL.Migrations
{
    public partial class add_ship_location : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHorizontal",
                table: "Ships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StartCol",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartRow",
                table: "Ships",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHorizontal",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "StartCol",
                table: "Ships");

            migrationBuilder.DropColumn(
                name: "StartRow",
                table: "Ships");
        }
    }
}
