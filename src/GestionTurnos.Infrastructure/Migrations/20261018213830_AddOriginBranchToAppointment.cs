using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginBranchToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OriginBranchId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginBranchId",
                table: "Appointments");
        }
    }
}
