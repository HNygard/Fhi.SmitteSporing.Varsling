using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class FlereNyeEgenskaperFraSmittekontaktrapport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InteressepunkterJson",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Enhetsinfo",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InteressepunkterJson",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "Enhetsinfo",
                schema: "Simula",
                table: "Smittekontakt");
        }
    }
}
