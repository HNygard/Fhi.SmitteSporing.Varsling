using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class SammenslaingTilIndekspasient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittetilfelle_Pasient_PasientId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropIndex(
                name: "IX_Smittetilfelle_PasientId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropColumn(
                name: "HentetFraKontaktInfo",
                schema: "Krr",
                table: "Telefon");

            migrationBuilder.DropColumn(
                name: "SmsMalId",
                schema: "App",
                table: "Kommune");

            migrationBuilder.AddColumn<string>(
                name: "Fodselsnummer",
                schema: "Msis",
                table: "Smittetilfelle",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TelefonId",
                schema: "Msis",
                table: "Smittetilfelle",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE s
                SET s.[TelefonId] = p.[TelefonId], s.[Fodselsnummer] = p.[Fodselsnummer]
                FROM [Msis].[Smittetilfelle] s INNER JOIN [Msis].[Pasient] p ON s.[PasientId] = p.[PasientId];
            ");

            migrationBuilder.DropTable(
                name: "Pasient",
                schema: "Msis");

            migrationBuilder.DropColumn(
                name: "PasientId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.CreateIndex(
                name: "IX_Smittetilfelle_TelefonId",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "TelefonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittetilfelle_Telefon_TelefonId",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "TelefonId",
                principalSchema: "Krr",
                principalTable: "Telefon",
                principalColumn: "TelefonId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Pasient",
                schema: "Msis",
                columns: table => new
                {
                    PasientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fodselsnummer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelefonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pasient", x => x.PasientId);
                    table.ForeignKey(
                        name: "FK_Pasient_Telefon_TelefonId",
                        column: x => x.TelefonId,
                        principalSchema: "Krr",
                        principalTable: "Telefon",
                        principalColumn: "TelefonId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<int>(
                name: "PasientId",
                schema: "Msis",
                table: "Smittetilfelle",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                INSERT INTO [Msis].[Pasient] ([Fodselsnummer], [Created])
                SELECT DISTINCT [Fodselsnummer], getdate() FROM [Msis].[Smittetilfelle];
            ");

            migrationBuilder.Sql(@"
                UPDATE s
                SET s.[PasientId] = p.[PasientId]
                FROM [Msis].[Smittetilfelle] s INNER JOIN [Msis].[Pasient] p ON s.[Fodselsnummer] = p.[Fodselsnummer];
            ");

            migrationBuilder.Sql(@"
                UPDATE p
                SET p.[TelefonId] = s.[TelefonId]
                FROM [Msis].[Pasient] p INNER JOIN [Msis].[Smittetilfelle] s ON p.[PasientId] = s.[PasientId];
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Smittetilfelle_Telefon_TelefonId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropIndex(
                name: "IX_Smittetilfelle_TelefonId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropColumn(
                name: "Fodselsnummer",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropColumn(
                name: "TelefonId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.AddColumn<DateTime>(
                name: "HentetFraKontaktInfo",
                schema: "Krr",
                table: "Telefon",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SmsMalId",
                schema: "App",
                table: "Kommune",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Smittetilfelle_PasientId",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "PasientId");

            migrationBuilder.CreateIndex(
                name: "IX_Pasient_TelefonId",
                schema: "Msis",
                table: "Pasient",
                column: "TelefonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittetilfelle_Pasient_PasientId",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "PasientId",
                principalSchema: "Msis",
                principalTable: "Pasient",
                principalColumn: "PasientId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
