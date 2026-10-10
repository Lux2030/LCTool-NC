using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCTool_NC.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NonConformita",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Stato = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Aperta"),
                    IsoNormaRiferimento = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ItemRiferimento = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RilevataDa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataApertura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataPrevistaChiusura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataEffettivaChiusura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Descrizione = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Causa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Risoluzione = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AzioniCorrettive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocSgqModificati = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DipartimentoCoinvolto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PartiInteressate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cliente = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Sedi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataRevisione = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NoteRevisione = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatoDa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModificatoDa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RevisionatoDa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonConformita", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonConformitaPartiInteressate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valore = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ordine = table.Column<int>(type: "int", nullable: false),
                    FlagAttivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonConformitaPartiInteressate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonConformitaRilevataDa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valore = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ordine = table.Column<int>(type: "int", nullable: false),
                    FlagAttivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonConformitaRilevataDa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonConformitaStati",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FlagAttivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonConformitaStati", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NonConformita_Numero",
                table: "NonConformita",
                column: "Numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NonConformita");

            migrationBuilder.DropTable(
                name: "NonConformitaPartiInteressate");

            migrationBuilder.DropTable(
                name: "NonConformitaRilevataDa");

            migrationBuilder.DropTable(
                name: "NonConformitaStati");
        }
    }
}
