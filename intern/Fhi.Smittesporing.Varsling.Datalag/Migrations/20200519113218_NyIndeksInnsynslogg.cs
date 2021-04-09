using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class NyIndeksInnsynslogg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Hva",
                schema: "Innsyn",
                table: "Innsyn",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Innsyn_Hva",
                schema: "Innsyn",
                table: "Innsyn",
                column: "Hva");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Innsyn_Hva",
                schema: "Innsyn",
                table: "Innsyn");

            migrationBuilder.AlterColumn<string>(
                name: "Hva",
                schema: "Innsyn",
                table: "Innsyn",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
