using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientNameSnapshotToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE a SET a.ClientName = c.Name FROM Appointments a JOIN Clients c ON c.Id = a.ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "Appointments");
        }
    }
}
