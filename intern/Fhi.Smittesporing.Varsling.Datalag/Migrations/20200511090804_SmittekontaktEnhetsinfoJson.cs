using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class SmittekontaktEnhetsinfoJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Enhetsinfo",
                schema: "Simula",
                table: "Smittekontakt",
                newName: "EnhetsinfoJson");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnhetsinfoJson",
                schema: "Simula",
                table: "Smittekontakt",
                newName: "Enhetsinfo");
        }
    }
}
