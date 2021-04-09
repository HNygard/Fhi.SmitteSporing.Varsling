using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class SmsVarselOgKoblingKontaktTilTelefon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TelefonId",
                schema: "Simula",
                table: "Smittekontakt",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Telefonnummer",
                schema: "KRR",
                table: "Telefon",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SmsVarsel",
                columns: table => new
                {
                    SmsVarselId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Referanse = table.Column<Guid>(nullable: true),
                    SmittekontaktId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsVarsel", x => x.SmsVarselId);
                    table.ForeignKey(
                        name: "FK_SmsVarsel_Smittekontakt_SmittekontaktId",
                        column: x => x.SmittekontaktId,
                        principalSchema: "Simula",
                        principalTable: "Smittekontakt",
                        principalColumn: "SmittekontaktId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                INSERT INTO [KRR].[Telefon] ([Telefonnummer],[Created])
                SELECT [Telefonnummer], getdate() FROM [Simula].[Smittekontakt] s
                WHERE NOT EXISTS (SELECT Null FROM [KRR].[Telefon] t WHERE t.[Telefonnummer] = s.[Telefonnummer])
                GROUP BY s.[Telefonnummer];

                UPDATE s
                SET s.[TelefonId] = t.[TelefonId]
                FROM [Simula].[Smittekontakt] s INNER JOIN [KRR].[Telefon] t ON s.[Telefonnummer] = t.[Telefonnummer];
            ");

            migrationBuilder.DropColumn(
                name: "Telefonnummer",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.CreateIndex(
                name: "IX_Smittekontakt_TelefonId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "TelefonId");

            migrationBuilder.CreateIndex(
                name: "IX_Telefon_Telefonnummer",
                schema: "KRR",
                table: "Telefon",
                column: "Telefonnummer",
                unique: true,
                filter: "[Telefonnummer] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SmsVarsel_Referanse",
                table: "SmsVarsel",
                column: "Referanse",
                unique: true,
                filter: "[Referanse] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SmsVarsel_SmittekontaktId",
                table: "SmsVarsel",
                column: "SmittekontaktId");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittekontakt_Telefon_TelefonId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "TelefonId",
                principalSchema: "KRR",
                principalTable: "Telefon",
                principalColumn: "TelefonId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittekontakt_Telefon_TelefonId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropTable(
                name: "SmsVarsel");

            migrationBuilder.DropIndex(
                name: "IX_Smittekontakt_TelefonId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropIndex(
                name: "IX_Telefon_Telefonnummer",
                schema: "KRR",
                table: "Telefon");

            migrationBuilder.AddColumn<string>(
                name: "Telefonnummer",
                schema: "Simula",
                table: "Smittekontakt",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE s
                SET s.[Telefonnummer] = t.[Telefonnummer]
                FROM [Simula].[Smittekontakt] s INNER JOIN [KRR].[Telefon] t ON t.[TelefonId] = s.[TelefonId];
            ");

            migrationBuilder.DropColumn(
                name: "TelefonId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.AlterColumn<string>(
                name: "Telefonnummer",
                schema: "KRR",
                table: "Telefon",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
