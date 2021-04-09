using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class UtvidetSmittekontaktInfoFraSimula : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Smittekontakter må hentes på nytt
            migrationBuilder.Sql(@"
                DELETE FROM [Simula].[Smittekontakt];
                UPDATE [Msis].[Indekspasient] SET [Status] = 0 WHERE [Status] = 4;
            ");

            migrationBuilder.DropColumn(
                name: "FraTidspunkt",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "Risikoverdi",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "TilTidspunkt",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "AkkumulertRisikoverdi",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "AkkumulertVarighet",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.AddColumn<double>(
                name: "BluetoothAkkumulertRisiko",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothAkkumulertVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "BluetoothInteressepunkterJson",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothMedianavstand",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Dato",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "GpsAkkumulertRisiko",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GpsAkkumulertVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "GpsInteressepunkterJson",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GpsMedianavstand",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OppsummertPlotHtmlId",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AntallKontakter",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothAkkumulertRisikoscore",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BluetoothAkkumulertVarighet",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GpsAkkumulertRisikoscore",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "GpsAkkumulertVarighet",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "InteressepunkterJson",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Risikokategori",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SmittekontaktDetaljerHtmlKart",
                schema: "Simula",
                columns: table => new
                {
                    SmittekontaktDetaljerHtmlKartId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmittekontaktDetaljerId = table.Column<int>(nullable: false),
                    Innhold = table.Column<string>(nullable: true),
                    Kryptert = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmittekontaktDetaljerHtmlKart", x => x.SmittekontaktDetaljerHtmlKartId);
                    table.ForeignKey(
                        name: "FK_SmittekontaktDetaljerHtmlKart_SmittekontaktDetaljer_SmittekontaktDetaljerId",
                        column: x => x.SmittekontaktDetaljerId,
                        principalSchema: "Simula",
                        principalTable: "SmittekontaktDetaljer",
                        principalColumn: "SmittekontaktDetaljerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmittekontaktDiagram",
                schema: "Simula",
                columns: table => new
                {
                    SmittekontaktDiagramId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmittekontaktId = table.Column<int>(nullable: false),
                    Data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmittekontaktDiagram", x => x.SmittekontaktDiagramId);
                    table.ForeignKey(
                        name: "FK_SmittekontaktDiagram_Smittekontakt_SmittekontaktId",
                        column: x => x.SmittekontaktId,
                        principalSchema: "Simula",
                        principalTable: "Smittekontakt",
                        principalColumn: "SmittekontaktId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmittekontaktDetaljerHtmlKart_SmittekontaktDetaljerId",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart",
                column: "SmittekontaktDetaljerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SmittekontaktDiagram_SmittekontaktId",
                schema: "Simula",
                table: "SmittekontaktDiagram",
                column: "SmittekontaktId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Smittekontakter må hentes på nytt
            migrationBuilder.Sql(@"
                DELETE FROM [Simula].[Smittekontakt];
                UPDATE [Msis].[Indekspasient] SET [Status] = 0 WHERE [Status] = 4;
            ");

            migrationBuilder.DropTable(
                name: "SmittekontaktDetaljerHtmlKart",
                schema: "Simula");

            migrationBuilder.DropTable(
                name: "SmittekontaktDiagram",
                schema: "Simula");

            migrationBuilder.DropColumn(
                name: "BluetoothAkkumulertRisiko",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "BluetoothAkkumulertVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "BluetoothInteressepunkterJson",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "BluetoothMedianavstand",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "Dato",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "GpsAkkumulertRisiko",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "GpsAkkumulertVarighet",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "GpsInteressepunkterJson",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "GpsMedianavstand",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "OppsummertPlotHtmlId",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "AntallKontakter",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "BluetoothAkkumulertRisikoscore",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "BluetoothAkkumulertVarighet",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "GpsAkkumulertRisikoscore",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "GpsAkkumulertVarighet",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "InteressepunkterJson",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "Risikokategori",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.AddColumn<DateTime>(
                name: "FraTidspunkt",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "Risikoverdi",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TilTidspunkt",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "AkkumulertRisikoverdi",
                schema: "Simula",
                table: "Smittekontakt",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AkkumulertVarighet",
                schema: "Simula",
                table: "Smittekontakt",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
