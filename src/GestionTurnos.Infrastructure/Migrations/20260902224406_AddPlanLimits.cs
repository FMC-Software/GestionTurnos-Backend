using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxBranchesAllowed",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "MaxServicesAllowed",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "MaxStaffAllowed",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: -1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxBranchesAllowed",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "MaxServicesAllowed",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "MaxStaffAllowed",
                table: "Plans");
        }
    }
}
