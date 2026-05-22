using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCTool_NC.Migrations
{
  
    public partial class AddPartiInteressate : Migration
    {
     
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

      
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NonConformitaPartiInteressate");
        }
    }
}
