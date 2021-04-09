using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class FjernetTesting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Testing",
                schema: "MSIS",
                table: "Smittetilfelle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Testing",
                schema: "MSIS",
                table: "Smittetilfelle",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
