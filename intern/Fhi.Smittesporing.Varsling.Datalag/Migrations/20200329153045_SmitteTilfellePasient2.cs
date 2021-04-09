using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class SmitteTilfellePasient2 : Migration
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
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
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
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Smittetilfelle_Pasient_PasientId",
                schema: "MSIS",
                table: "Smittetilfelle",
                column: "PasientId",
                principalTable: "Pasient",
                principalColumn: "PasientId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
