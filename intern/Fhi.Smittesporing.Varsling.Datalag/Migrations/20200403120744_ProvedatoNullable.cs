using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class ProvedatoNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Provedato",
                schema: "MSIS",
                table: "Smittetilfelle",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Provedato",
                schema: "MSIS",
                table: "Smittetilfelle",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
