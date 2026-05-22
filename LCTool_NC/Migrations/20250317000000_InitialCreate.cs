using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCTool_NC.Migrations
{
    
    public partial class InitialCreate : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "NonConformita");
        }
    }
}