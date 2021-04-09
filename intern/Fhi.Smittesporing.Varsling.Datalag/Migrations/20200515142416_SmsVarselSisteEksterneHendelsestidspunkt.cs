using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class SmsVarselSisteEksterneHendelsestidspunkt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SisteEksterneHendelsestidspunkt",
                schema: "Sms",
                table: "SmsVarsel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SisteEksterneHendelsestidspunkt",
                schema: "Sms",
                table: "SmsVarsel");
        }
    }
}
