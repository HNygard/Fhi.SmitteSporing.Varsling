using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class KommuneSmsFletteinfoFjernetIndekspasientHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Oppdatering ifm utgåtte statuser
            migrationBuilder.Sql(@"
                UPDATE [Msis].[Indekspasient] SET Status = 1 WHERE Status IN (5, 6);
            ");

            migrationBuilder.DropColumn(
                name: "Sha256",
                schema: "Msis",
                table: "Indekspasient");

            migrationBuilder.AddColumn<string>(
                name: "SmsFletteinfo",
                schema: "App",
                table: "Kommune",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SmsFletteinfo",
                schema: "App",
                table: "Kommune");

            migrationBuilder.AddColumn<string>(
                name: "Sha256",
                schema: "Msis",
                table: "Indekspasient",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
