using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutSectionToLandingContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutDescriptionEn",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutDescriptionEs",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutTitleEn",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutTitleEs",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutDescriptionEn",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "AboutDescriptionEs",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "AboutTitleEn",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "AboutTitleEs",
                table: "LandingContents");
        }
    }
}
