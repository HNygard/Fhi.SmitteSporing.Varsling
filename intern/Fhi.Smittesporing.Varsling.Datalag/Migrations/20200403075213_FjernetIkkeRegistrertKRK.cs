using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class FjernetIkkeRegistrertKRK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IkkeRegistrertKRK",
                table: "Pasient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IkkeRegistrertKRK",
                table: "Pasient",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
