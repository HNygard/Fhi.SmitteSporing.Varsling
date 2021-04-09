using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class FjernetUbruktKolonne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kryptert",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Kryptert",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
