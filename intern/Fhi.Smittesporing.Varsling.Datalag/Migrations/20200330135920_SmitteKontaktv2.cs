using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class SmitteKontaktv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelefonId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.AddColumn<string>(
                name: "Telefonnummer",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Smittekontakt_SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "SmittetilfelleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittekontakt_Smittetilfelle_SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "SmittetilfelleId",
                principalSchema: "MSIS",
                principalTable: "Smittetilfelle",
                principalColumn: "SmittetilfelleId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittekontakt_Smittetilfelle_SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropIndex(
                name: "IX_Smittekontakt_SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "Telefonnummer",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.AddColumn<int>(
                name: "TelefonId",
                schema: "Simula",
                table: "Smittekontakt",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
