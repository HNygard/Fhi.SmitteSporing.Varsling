using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class PasientTelefon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pasient_TelefonId",
                table: "Pasient",
                column: "TelefonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pasient_Telefon_TelefonId",
                table: "Pasient",
                column: "TelefonId",
                principalSchema: "KRR",
                principalTable: "Telefon",
                principalColumn: "TelefonId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pasient_Telefon_TelefonId",
                table: "Pasient");

            migrationBuilder.DropIndex(
                name: "IX_Pasient_TelefonId",
                table: "Pasient");
        }
    }
}
