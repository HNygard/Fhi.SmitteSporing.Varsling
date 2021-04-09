using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class LagtTilKommunePaSmittetilfelle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KommuneId",
                schema: "Msis",
                table: "Smittetilfelle",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KommuneNr",
                schema: "App",
                table: "Kommune",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Smittetilfelle_KommuneId",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "KommuneId");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittetilfelle_Kommune_KommuneId",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "KommuneId",
                principalSchema: "App",
                principalTable: "Kommune",
                principalColumn: "KommuneId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittetilfelle_Kommune_KommuneId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropIndex(
                name: "IX_Smittetilfelle_KommuneId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropColumn(
                name: "KommuneId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropColumn(
                name: "KommuneNr",
                schema: "App",
                table: "Kommune");
        }
    }
}
