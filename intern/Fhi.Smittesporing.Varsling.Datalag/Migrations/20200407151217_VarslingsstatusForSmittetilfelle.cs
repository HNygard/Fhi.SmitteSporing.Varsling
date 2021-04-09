using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class VarslingsstatusForSmittetilfelle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Varslingsstatus",
                schema: "Msis",
                table: "Smittetilfelle",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Varslingsstatus",
                schema: "Msis",
                table: "Smittetilfelle");
        }
    }
}
