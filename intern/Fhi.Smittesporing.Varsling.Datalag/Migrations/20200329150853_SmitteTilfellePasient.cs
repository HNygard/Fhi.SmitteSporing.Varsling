using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class SmitteTilfellePasient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PasientId",
                schema: "MSIS",
                table: "Smittetilfelle",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Smittetilfelle_PasientId",
                schema: "MSIS",
                table: "Smittetilfelle",
                column: "PasientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittetilfelle_Pasient_PasientId",
                schema: "MSIS",
                table: "Smittetilfelle",
                column: "PasientId",
                principalTable: "Pasient",
                principalColumn: "PasientId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittetilfelle_Pasient_PasientId",
                schema: "MSIS",
                table: "Smittetilfelle");

            migrationBuilder.DropIndex(
                name: "IX_Smittetilfelle_PasientId",
                schema: "MSIS",
                table: "Smittetilfelle");

            migrationBuilder.DropColumn(
                name: "PasientId",
                schema: "MSIS",
                table: "Smittetilfelle");
        }
    }
}
