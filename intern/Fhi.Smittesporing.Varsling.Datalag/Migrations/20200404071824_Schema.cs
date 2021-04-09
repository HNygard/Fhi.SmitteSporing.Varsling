using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class Schema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "App");

            migrationBuilder.EnsureSchema(
                name: "Innsyn");

            migrationBuilder.EnsureSchema(
                name: "Msis");

            migrationBuilder.EnsureSchema(
                name: "Sms");

            migrationBuilder.EnsureSchema(
                name: "Krr");

            migrationBuilder.RenameTable(
                name: "Smittetilfelle",
                schema: "MSIS",
                newName: "Smittetilfelle",
                newSchema: "Msis");

            migrationBuilder.RenameTable(
                name: "Telefon",
                schema: "KRR",
                newName: "Telefon",
                newSchema: "Krr");

            migrationBuilder.RenameTable(
                name: "SmsVarsel",
                newName: "SmsVarsel",
                newSchema: "Sms");

            migrationBuilder.RenameTable(
                name: "Pasient",
                newName: "Pasient",
                newSchema: "Msis");

            migrationBuilder.RenameTable(
                name: "Innsyn",
                newName: "Innsyn",
                newSchema: "Innsyn");

            migrationBuilder.RenameTable(
                name: "Applikasjonsinnstilling",
                newName: "Applikasjonsinnstilling",
                newSchema: "App");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MSIS");

            migrationBuilder.EnsureSchema(
                name: "KRR");

            migrationBuilder.RenameTable(
                name: "Smittetilfelle",
                schema: "Msis",
                newName: "Smittetilfelle",
                newSchema: "MSIS");

            migrationBuilder.RenameTable(
                name: "Telefon",
                schema: "Krr",
                newName: "Telefon",
                newSchema: "KRR");

            migrationBuilder.RenameTable(
                name: "SmsVarsel",
                schema: "Sms",
                newName: "SmsVarsel");

            migrationBuilder.RenameTable(
                name: "Pasient",
                schema: "Msis",
                newName: "Pasient");

            migrationBuilder.RenameTable(
                name: "Innsyn",
                schema: "Innsyn",
                newName: "Innsyn");

            migrationBuilder.RenameTable(
                name: "Applikasjonsinnstilling",
                schema: "App",
                newName: "Applikasjonsinnstilling");
        }
    }
}
