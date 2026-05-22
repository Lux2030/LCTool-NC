using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCTool_NC.Migrations
{
    public partial class RenameUtentiNCToUtentiNonConformita : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UtentiNC",
                newName: "UtentiNonConformita");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UtentiNonConformita",
                newName: "UtentiNC");
        }
    }
}