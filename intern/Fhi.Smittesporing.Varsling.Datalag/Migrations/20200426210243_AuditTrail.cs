using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class AuditTrail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OppdatertTidspunkt",
                schema: "Sms",
                table: "SmsVarsel",
                newName: "SistOppdatert");

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "Sms",
                table: "SmsVarsel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "Sms",
                table: "SmsVarsel",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "Simula",
                table: "SmittekontaktDiagram",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "Simula",
                table: "SmittekontaktDiagram",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SistOppdatert",
                schema: "Simula",
                table: "SmittekontaktDiagram",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "Simula",
                table: "SmittekontaktDiagram",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SistOppdatert",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SistOppdatert",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "Simula",
                table: "SmittekontaktDetaljer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SistOppdatert",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "Msis",
                table: "Indekspasient",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SistOppdatert",
                schema: "Msis",
                table: "Indekspasient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "Msis",
                table: "Indekspasient",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "Krr",
                table: "Telefon",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SistOppdatert",
                schema: "Krr",
                table: "Telefon",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "Krr",
                table: "Telefon",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "App",
                table: "Kommune",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "App",
                table: "Kommune",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SistOppdatert",
                schema: "App",
                table: "Kommune",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "App",
                table: "Kommune",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "App",
                table: "Applikasjonsinnstilling",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OpprettetAv",
                schema: "App",
                table: "Applikasjonsinnstilling",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SistOppdatert",
                schema: "App",
                table: "Applikasjonsinnstilling",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistOppdatertAv",
                schema: "App",
                table: "Applikasjonsinnstilling",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditTrail",
                schema: "App",
                columns: table => new
                {
                    AuditTrailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Modell = table.Column<string>(nullable: true),
                    PrimarNokkel = table.Column<string>(nullable: true),
                    Egenskap = table.Column<string>(nullable: true),
                    NyVerdi = table.Column<string>(nullable: true),
                    GammelVerdi = table.Column<string>(nullable: true),
                    Tidspunkt = table.Column<DateTime>(nullable: false),
                    UtfortAv = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrail", x => x.AuditTrailId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrail",
                schema: "App");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "Sms",
                table: "SmsVarsel");

            migrationBuilder.RenameColumn(
                name: "SistOppdatert",
                schema: "Sms",
                table: "SmsVarsel",
                newName: "OppdatertTidspunkt");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "Sms",
                table: "SmsVarsel");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "Simula",
                table: "SmittekontaktDiagram");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "Simula",
                table: "SmittekontaktDiagram");

            migrationBuilder.DropColumn(
                name: "SistOppdatert",
                schema: "Simula",
                table: "SmittekontaktDiagram");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "Simula",
                table: "SmittekontaktDiagram");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart");

            migrationBuilder.DropColumn(
                name: "SistOppdatert",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "Simula",
                table: "SmittekontaktDetaljerHtmlKart");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "SistOppdatert",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "Simula",
                table: "SmittekontaktDetaljer");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "SistOppdatert",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "Msis",
                table: "Indekspasient");

            migrationBuilder.DropColumn(
                name: "SistOppdatert",
                schema: "Msis",
                table: "Indekspasient");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "Msis",
                table: "Indekspasient");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "Krr",
                table: "Telefon");

            migrationBuilder.DropColumn(
                name: "SistOppdatert",
                schema: "Krr",
                table: "Telefon");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "Krr",
                table: "Telefon");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "App",
                table: "Kommune");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "App",
                table: "Kommune");

            migrationBuilder.DropColumn(
                name: "SistOppdatert",
                schema: "App",
                table: "Kommune");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "App",
                table: "Kommune");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "App",
                table: "Applikasjonsinnstilling");

            migrationBuilder.DropColumn(
                name: "OpprettetAv",
                schema: "App",
                table: "Applikasjonsinnstilling");

            migrationBuilder.DropColumn(
                name: "SistOppdatert",
                schema: "App",
                table: "Applikasjonsinnstilling");

            migrationBuilder.DropColumn(
                name: "SistOppdatertAv",
                schema: "App",
                table: "Applikasjonsinnstilling");
        }
    }
}
