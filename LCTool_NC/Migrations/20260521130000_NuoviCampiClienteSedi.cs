using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCTool_NC.Migrations
{
    /// <inheritdoc />
    public partial class NuoviCampiClienteSedi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cliente",
                table: "NonConformita",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sedi",
                table: "NonConformita",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            // Aggiunta voce "Audit Interno" in RilevataDa (idempotente)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM [NonConformitaRilevataDa] WHERE [Valore] = 'Audit Interno')
                BEGIN
                    INSERT INTO [NonConformitaRilevataDa] ([Valore], [Ordine], [FlagAttivo])
                    VALUES ('Audit Interno',
                        (SELECT ISNULL(MAX([Ordine]),0)+1 FROM [NonConformitaRilevataDa]), 1);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cliente",
                table: "NonConformita");

            migrationBuilder.DropColumn(
                name: "Sedi",
                table: "NonConformita");

            migrationBuilder.Sql(
                "DELETE FROM [NonConformitaRilevataDa] WHERE [Valore] = 'Audit Interno';");
        }
    }
}
