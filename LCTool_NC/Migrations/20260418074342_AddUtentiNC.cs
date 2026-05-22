using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCTool_NC.Migrations
{
    
    public partial class AddUtentiNC : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UtentiNC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pin = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    FlagAttivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtentiNC", x => x.Id);
                });
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UtentiNC");
        }
    }
}
