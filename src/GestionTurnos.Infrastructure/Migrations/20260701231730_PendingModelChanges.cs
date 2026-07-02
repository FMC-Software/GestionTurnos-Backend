using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Verificar si la constraint existe antes de dropearla
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Users_Branches_BranchId1' AND parent_object_id = OBJECT_ID('Users'))
                BEGIN
                    ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Branches_BranchId1];
                END
            ");

            // Verificar si el índice existe antes de dropearlo
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_BranchId1' AND object_id = OBJECT_ID('Users'))
                BEGIN
                    DROP INDEX [IX_Users_BranchId1] ON [Users];
                END
            ");

            // Verificar si la columna existe antes de dropearla
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'BranchId1')
                BEGIN
                    ALTER TABLE [Users] DROP COLUMN [BranchId1];
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId1",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BranchId1",
                table: "Users",
                column: "BranchId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Branches_BranchId1",
                table: "Users",
                column: "BranchId1",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
