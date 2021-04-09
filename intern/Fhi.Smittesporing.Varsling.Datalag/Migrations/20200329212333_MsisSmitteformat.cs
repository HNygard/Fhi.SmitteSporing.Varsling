﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class MsisSmitteformat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Opprettettidspunkt",
                schema: "MSIS",
                table: "Smittetilfelle",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Provedato",
                schema: "MSIS",
                table: "Smittetilfelle",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Opprettettidspunkt",
                schema: "MSIS",
                table: "Smittetilfelle");

            migrationBuilder.DropColumn(
                name: "Provedato",
                schema: "MSIS",
                table: "Smittetilfelle");
        }
    }
}
