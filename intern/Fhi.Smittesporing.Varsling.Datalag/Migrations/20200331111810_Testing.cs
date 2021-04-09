using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class Testing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Testing",
                schema: "MSIS",
                table: "Smittetilfelle",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Testing",
                schema: "MSIS",
                table: "Smittetilfelle");
        }
    }
}
