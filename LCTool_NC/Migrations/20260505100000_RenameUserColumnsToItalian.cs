using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LCTool_NC.Migrations
{
   
    public partial class RenameUserColumnsToItalian : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- CreatedUser -> CreatoDa
                IF EXISTS (SELECT 1 FROM sys.columns
                           WHERE object_id = OBJECT_ID('NonConformita') AND name = 'CreatedUser')
                   AND NOT EXISTS (SELECT 1 FROM sys.columns
                                   WHERE object_id = OBJECT_ID('NonConformita') AND name = 'CreatoDa')
                BEGIN
                    EXEC sp_rename 'NonConformita.CreatedUser', 'CreatoDa', 'COLUMN';
                END
                ELSE IF NOT EXISTS (SELECT 1 FROM sys.columns
                                    WHERE object_id = OBJECT_ID('NonConformita') AND name = 'CreatoDa')
                BEGIN
                    ALTER TABLE [NonConformita] ADD [CreatoDa] nvarchar(100) NULL;
                END

                -- UpdatedUser -> ModificatoDa
                IF EXISTS (SELECT 1 FROM sys.columns
                           WHERE object_id = OBJECT_ID('NonConformita') AND name = 'UpdatedUser')
                   AND NOT EXISTS (SELECT 1 FROM sys.columns
                                   WHERE object_id = OBJECT_ID('NonConformita') AND name = 'ModificatoDa')
                BEGIN
                    EXEC sp_rename 'NonConformita.UpdatedUser', 'ModificatoDa', 'COLUMN';
                END
                ELSE IF NOT EXISTS (SELECT 1 FROM sys.columns
                                    WHERE object_id = OBJECT_ID('NonConformita') AND name = 'ModificatoDa')
                BEGIN
                    ALTER TABLE [NonConformita] ADD [ModificatoDa] nvarchar(100) NULL;
                END

                -- Nuova colonna RevisionatoDa
                IF NOT EXISTS (SELECT 1 FROM sys.columns
                               WHERE object_id = OBJECT_ID('NonConformita') AND name = 'RevisionatoDa')
                BEGIN
                    ALTER TABLE [NonConformita] ADD [RevisionatoDa] nvarchar(100) NULL;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns
                           WHERE object_id = OBJECT_ID('NonConformita') AND name = 'RevisionatoDa')
                BEGIN
                    ALTER TABLE [NonConformita] DROP COLUMN [RevisionatoDa];
                END

                IF EXISTS (SELECT 1 FROM sys.columns
                           WHERE object_id = OBJECT_ID('NonConformita') AND name = 'ModificatoDa')
                   AND NOT EXISTS (SELECT 1 FROM sys.columns
                                   WHERE object_id = OBJECT_ID('NonConformita') AND name = 'UpdatedUser')
                BEGIN
                    EXEC sp_rename 'NonConformita.ModificatoDa', 'UpdatedUser', 'COLUMN';
                END

                IF EXISTS (SELECT 1 FROM sys.columns
                           WHERE object_id = OBJECT_ID('NonConformita') AND name = 'CreatoDa')
                   AND NOT EXISTS (SELECT 1 FROM sys.columns
                                   WHERE object_id = OBJECT_ID('NonConformita') AND name = 'CreatedUser')
                BEGIN
                    EXEC sp_rename 'NonConformita.CreatoDa', 'CreatedUser', 'COLUMN';
                END
            ");
        }
    }
}
