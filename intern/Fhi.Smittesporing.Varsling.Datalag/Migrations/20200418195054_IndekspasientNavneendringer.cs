using Microsoft.EntityFrameworkCore.Migrations;

namespace Fhi.Smittesporing.Varsling.Datalag.Migrations
{
    public partial class IndekspasientNavneendringer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Smittetilfelle_Kommune_KommuneId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropForeignKey(
                name: "FK_Smittetilfelle_Telefon_TelefonId",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.DropForeignKey(
                name: "FK_Smittekontakt_Smittetilfelle_SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropIndex(
                name: "IX_Smittekontakt_SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Smittetilfelle",
                schema: "Msis",
                table: "Smittetilfelle");

            migrationBuilder.RenameTable(
                name: "Smittetilfelle",
                schema: "Msis",
                newName: "Indekspasient",
                newSchema: "Msis");

            migrationBuilder.RenameIndex(
                name: "IX_Smittetilfelle_TelefonId",
                schema: "Msis",
                table: "Indekspasient",
                newName: "IX_Indekspasient_TelefonId");

            migrationBuilder.RenameIndex(
                name: "IX_Smittetilfelle_KommuneId",
                schema: "Msis",
                table: "Indekspasient",
                newName: "IX_Indekspasient_KommuneId");

            migrationBuilder.RenameColumn(
                name: "SmittetilfelleId",
                schema: "Msis",
                table: "Indekspasient",
                newName: "IndekspasientId");

            migrationBuilder.RenameColumn(
                name: "SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt",
                newName: "IndekspasientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Indekspasient",
                schema: "Msis",
                table: "Indekspasient",
                column: "IndekspasientId");

            migrationBuilder.CreateIndex(
                name: "IX_Smittekontakt_IndekspasientId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "IndekspasientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Indekspasient_Kommune_KommuneId",
                schema: "Msis",
                table: "Indekspasient",
                column: "KommuneId",
                principalSchema: "App",
                principalTable: "Kommune",
                principalColumn: "KommuneId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Indekspasient_Telefon_TelefonId",
                schema: "Msis",
                table: "Indekspasient",
                column: "TelefonId",
                principalSchema: "Krr",
                principalTable: "Telefon",
                principalColumn: "TelefonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Smittekontakt_Indekspasient_IndekspasientId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "IndekspasientId",
                principalSchema: "Msis",
                principalTable: "Indekspasient",
                principalColumn: "IndekspasientId",
                onDelete: ReferentialAction.Cascade);


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Indekspasient_Kommune_KommuneId",
                schema: "Msis",
                table: "Indekspasient");

            migrationBuilder.DropForeignKey(
                name: "FK_Indekspasient_Telefon_TelefonId",
                schema: "Msis",
                table: "Indekspasient");

            migrationBuilder.DropForeignKey(
                name: "FK_Smittekontakt_Indekspasient_IndekspasientId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropIndex(
                name: "IX_Smittekontakt_IndekspasientId",
                schema: "Simula",
                table: "Smittekontakt");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Indekspasient",
                schema: "Msis",
                table: "Indekspasient");

            migrationBuilder.RenameTable(
                name: "Indekspasient",
                schema: "Msis",
                newName: "Smittetilfelle",
                newSchema: "Msis");

            migrationBuilder.RenameColumn(
                name: "IndekspasientId",
                schema: "Msis",
                table: "Smittetilfelle",
                newName: "SmittetilfelleId");

            migrationBuilder.RenameColumn(
                name: "IndekspasientId",
                schema: "Simula",
                table: "Smittekontakt",
                newName: "SmittetilfelleId");

            migrationBuilder.RenameIndex(
                name: "IX_Indekspasient_TelefonId",
                schema: "Msis",
                table: "Smittetilfelle",
                newName: "IX_Smittetilfelle_TelefonId");

            migrationBuilder.RenameIndex(
                name: "IX_Indekspasient_KommuneId",
                schema: "Msis",
                table: "Smittetilfelle",
                newName: "IX_Smittetilfelle_KommuneId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Smittetilfelle",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "SmittetilfelleId");

            migrationBuilder.CreateIndex(
                name: "IX_Smittekontakt_SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "SmittetilfelleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Smittetilfelle_Kommune_KommuneId",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "KommuneId",
                principalSchema: "App",
                principalTable: "Kommune",
                principalColumn: "KommuneId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Smittetilfelle_Telefon_TelefonId",
                schema: "Msis",
                table: "Smittetilfelle",
                column: "TelefonId",
                principalSchema: "Krr",
                principalTable: "Telefon",
                principalColumn: "TelefonId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Smittekontakt_Smittetilfelle_SmittetilfelleId",
                schema: "Simula",
                table: "Smittekontakt",
                column: "SmittetilfelleId",
                principalSchema: "Msis",
                principalTable: "Smittetilfelle",
                principalColumn: "SmittetilfelleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
