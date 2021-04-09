using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class NyKolonneVerifiseringskodeForSmittekontakt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Verifiseringskode",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Verifiseringskode",
                schema: "Simula",
                table: "Smittekontakt");
        }
    }
}
