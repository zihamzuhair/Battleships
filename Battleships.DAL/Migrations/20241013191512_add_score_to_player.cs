using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Battleships.DAL.Migrations
{
    public partial class add_score_to_player : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Players");
        }
    }
}
