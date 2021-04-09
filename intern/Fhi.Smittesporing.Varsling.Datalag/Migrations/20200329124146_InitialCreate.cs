using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Simula");

            migrationBuilder.EnsureSchema(
                name: "MSIS");

            migrationBuilder.EnsureSchema(
                name: "KRR");

            migrationBuilder.CreateTable(
                name: "Pasient",
                columns: table => new
                {
                    PasientId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fodselsnummer = table.Column<string>(nullable: true),
                    TelefonId = table.Column<int>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pasient", x => x.PasientId);
                });

            migrationBuilder.CreateTable(
                name: "Stedsinfo",
                columns: table => new
                {
                    StedsinfoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kommune = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stedsinfo", x => x.StedsinfoId);
                });

            migrationBuilder.CreateTable(
                name: "Telefon",
                schema: "KRR",
                columns: table => new
                {
                    TelefonId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Telefonnummer = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telefon", x => x.TelefonId);
                });

            migrationBuilder.CreateTable(
                name: "Smittetilfelle",
                schema: "MSIS",
                columns: table => new
                {
                    SmittetilfelleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fodselsnummer = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Smittetilfelle", x => x.SmittetilfelleId);
                });

            migrationBuilder.CreateTable(
                name: "Smittekontakt",
                schema: "Simula",
                columns: table => new
                {
                    SmittekontaktId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VarsletTidspunkt = table.Column<DateTime>(nullable: true),
                    SmittetilfelleId = table.Column<int>(nullable: false),
                    TelefonId = table.Column<int>(nullable: false),
                    StedsinfoId = table.Column<int>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Smittekontakt", x => x.SmittekontaktId);
                    table.ForeignKey(
                        name: "FK_Smittekontakt_Stedsinfo_StedsinfoId",
                        column: x => x.StedsinfoId,
                        principalTable: "Stedsinfo",
                        principalColumn: "StedsinfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Smittekontakt_StedsinfoId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "StedsinfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pasient");

            migrationBuilder.DropTable(
                name: "Telefon",
                schema: "KRR");

            migrationBuilder.DropTable(
                name: "Smittetilfelle",
                schema: "MSIS");

            migrationBuilder.DropTable(
                name: "Smittekontakt",
                schema: "Simula");

            migrationBuilder.DropTable(
                name: "Stedsinfo");
        }
    }
}
