using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class PasientKanMangle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittetilfelle_Pasient_PasientId",
                schema: "MSIS",
                table: "Smittetilfelle");

            migrationBuilder.AlterColumn<int>(
                name: "PasientId",
                schema: "MSIS",
                table: "Smittetilfelle",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AlterColumn<int>(
                name: "PasientId",
                schema: "MSIS",
                table: "Smittetilfelle",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Smittetilfelle_Pasient_PasientId",
                schema: "MSIS",
                table: "Smittetilfelle",
                column: "PasientId",
                principalTable: "Pasient",
                principalColumn: "PasientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
