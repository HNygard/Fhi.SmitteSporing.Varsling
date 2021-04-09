using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class NyeEgenskaperFraSmittekontaktrapport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AntallDagerMedKontakt",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BluetoothAntallDagerMedKontakt",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GpsAntallDagerMedKontakt",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PipelineVersjon",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AntallDagerMedKontakt",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "BluetoothAntallDagerMedKontakt",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "GpsAntallDagerMedKontakt",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "PipelineVersjon",
                schema: "Simula",
                table: "Smittekontakt");
        }
    }
}
