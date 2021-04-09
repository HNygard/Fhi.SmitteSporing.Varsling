using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class IkkeRegistrertKRK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HentetFraKontaktInfo",
                schema: "KRR",
                table: "Telefon",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IkkeRegistrertKRK",
                table: "Pasient",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Applikasjonsinnstilling",
                columns: table => new
                {
                    ApplikasjonsinnstillingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nokkel = table.Column<string>(nullable: true),
                    Verdi = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applikasjonsinnstilling", x => x.ApplikasjonsinnstillingId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applikasjonsinnstilling_Nokkel",
                table: "Applikasjonsinnstilling",
                column: "Nokkel",
                unique: true,
                filter: "[Nokkel] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Applikasjonsinnstilling");

            migrationBuilder.DropColumn(
                name: "HentetFraKontaktInfo",
                schema: "KRR",
                table: "Telefon");

            migrationBuilder.DropColumn(
                name: "IkkeRegistrertKRK",
                table: "Pasient");
        }
    }
}
