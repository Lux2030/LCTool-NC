using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCTool_NC.Migrations
{
    public partial class AddCreatedUpdatedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns 
                               WHERE object_id = OBJECT_ID('NonConformita') AND name = 'CreatedUser')
                    ALTER TABLE [NonConformita] ADD [CreatedUser] nvarchar(100) NULL;

                IF NOT EXISTS (SELECT 1 FROM sys.columns 
                               WHERE object_id = OBJECT_ID('NonConformita') AND name = 'UpdatedUser')
                    ALTER TABLE [NonConformita] ADD [UpdatedUser] nvarchar(100) NULL;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}