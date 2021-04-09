using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class KontaktdetaljerOgNyeKontaktegenskaper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<double>(
                name: "AkkumulertRisikoverdi",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AkkumulertVarighet",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "SmittekontaktDetaljer",
                schema: "Simula",
                columns: table => new
                {
                    SmittekontaktDetaljerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmittekontaktId = table.Column<int>(nullable: false),
                    FraTidspunkt = table.Column<DateTime>(nullable: false),
                    TilTidspunkt = table.Column<DateTime>(nullable: false),
                    Risikoverdi = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmittekontaktDetaljer", x => x.SmittekontaktDetaljerId);
                    table.ForeignKey(
                        name: "FK_SmittekontaktDetaljer_Smittekontakt_SmittekontaktId",
                        column: x => x.SmittekontaktId,
                        principalSchema: "Simula",
                        principalTable: "Smittekontakt",
                        principalColumn: "SmittekontaktId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmittekontaktDetaljer_SmittekontaktId",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                column: "SmittekontaktId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmittekontaktDetaljer",
                schema: "Simula");

            migrationBuilder.DropColumn(
                name: "AkkumulertRisikoverdi",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "AkkumulertVarighet",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.AddColumn<double>(
                name: "AccumulatedDistance",
                schema: "Simula",
                table: "Smittekontakt",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ExtraInfo",
                schema: "Simula",
                table: "Smittekontakt",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RiskScore",
                schema: "Simula",
                table: "Smittekontakt",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
