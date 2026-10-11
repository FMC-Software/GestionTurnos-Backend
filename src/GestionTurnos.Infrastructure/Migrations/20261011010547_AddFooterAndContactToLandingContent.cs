using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFooterAndContactToLandingContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterDescriptionEn",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FooterDescriptionEs",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkedinUrl",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TwitterUrl",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "FooterDescriptionEn",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "FooterDescriptionEs",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "LinkedinUrl",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "TwitterUrl",
                table: "LandingContents");
        }
    }
}
