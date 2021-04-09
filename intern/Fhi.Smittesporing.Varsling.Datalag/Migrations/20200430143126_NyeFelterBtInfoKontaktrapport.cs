using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class NyeFelterBtInfoKontaktrapport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BluetoothNarVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothRelativtNarVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothVeldigNarVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothNarVarighet",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothRelativtNarVarighet",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothVeldigNarVarighet",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BluetoothNarVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "BluetoothRelativtNarVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "BluetoothVeldigNarVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "BluetoothNarVarighet",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "BluetoothRelativtNarVarighet",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "BluetoothVeldigNarVarighet",
                schema: "Simula",
                table: "Smittekontakt");
        }
    }
}
