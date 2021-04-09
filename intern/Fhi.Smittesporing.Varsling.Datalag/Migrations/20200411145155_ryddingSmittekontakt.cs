using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class ryddingSmittekontakt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittekontakt_Stedsinfo_StedsinfoId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropTable(
                name: "Stedsinfo");

            migrationBuilder.DropIndex(
                name: "IX_Smittekontakt_StedsinfoId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "KontaktTidspunkt",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropColumn(
                name: "StedsinfoId",
                schema: "Simula",
                table: "Smittekontakt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "KontaktTidspunkt",
                schema: "Simula",
                table: "Smittekontakt",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StedsinfoId",
                schema: "Simula",
                table: "Smittekontakt",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stedsinfo",
                columns: table => new
                {
                    StedsinfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kommune = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stedsinfo", x => x.StedsinfoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Smittekontakt_StedsinfoId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "StedsinfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittekontakt_Stedsinfo_StedsinfoId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "StedsinfoId",
                principalTable: "Stedsinfo",
                principalColumn: "StedsinfoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
