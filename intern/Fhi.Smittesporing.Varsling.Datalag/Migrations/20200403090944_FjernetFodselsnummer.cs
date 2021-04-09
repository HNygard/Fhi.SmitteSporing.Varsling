using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class FjernetFodselsnummer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fodselsnummer",
                schema: "MSIS",
                table: "Smittetilfelle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fodselsnummer",
                schema: "MSIS",
                table: "Smittetilfelle",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
