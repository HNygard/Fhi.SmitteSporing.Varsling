using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class CascadeOgRestrictConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittekontakt_Telefon_TelefonId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittekontakt_Telefon_TelefonId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "TelefonId",
                principalSchema: "Krr",
                principalTable: "Telefon",
                principalColumn: "TelefonId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittekontakt_Telefon_TelefonId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittekontakt_Telefon_TelefonId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "TelefonId",
                principalSchema: "Krr",
                principalTable: "Telefon",
                principalColumn: "TelefonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
