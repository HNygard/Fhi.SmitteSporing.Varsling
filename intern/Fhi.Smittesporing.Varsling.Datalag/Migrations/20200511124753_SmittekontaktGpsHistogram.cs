using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class SmittekontaktGpsHistogram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmittekontaktGpsHistogram",
                schema: "Simula",
                columns: table => new
                {
                    SmittekontaktGpsHistogramId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmittekontaktId = table.Column<int>(nullable: false),
                    Data = table.Column<byte[]>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    OpprettetAv = table.Column<string>(nullable: true),
                    SistOppdatertAv = table.Column<string>(nullable: true),
                    SistOppdatert = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmittekontaktGpsHistogram", x => x.SmittekontaktGpsHistogramId);
                    table.ForeignKey(
                        name: "FK_SmittekontaktGpsHistogram_Smittekontakt_SmittekontaktId",
                        column: x => x.SmittekontaktId,
                        principalSchema: "Simula",
                        principalTable: "Smittekontakt",
                        principalColumn: "SmittekontaktId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmittekontaktGpsHistogram_SmittekontaktId",
                schema: "Simula",
                table: "SmittekontaktGpsHistogram",
                column: "SmittekontaktId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmittekontaktGpsHistogram",
                schema: "Simula");
        }
    }
}
