using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class AccumulatedDistance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AccumulatedDistance",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ExtraInfo",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RiskScore",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccumulatedDistance",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "ExtraInfo",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "RiskScore",
                schema: "Simula",
                table: "Smittekontakt");
        }
    }
}
