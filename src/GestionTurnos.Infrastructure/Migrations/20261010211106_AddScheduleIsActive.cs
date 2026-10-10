using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Antes un dia cerrado se guardaba como IsDeleted = true; ahora se representa con IsActive = false.
            migrationBuilder.Sql("UPDATE Schedules SET IsActive = CASE WHEN IsDeleted = 1 THEN 0 ELSE 1 END");
            migrationBuilder.Sql("UPDATE Schedules SET IsDeleted = 0, DeleteDateTime = NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Schedules SET IsDeleted = CASE WHEN IsActive = 0 THEN 1 ELSE 0 END");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Schedules");
        }
    }
}
