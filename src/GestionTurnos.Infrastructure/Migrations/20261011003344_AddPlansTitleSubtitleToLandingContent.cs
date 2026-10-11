using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlansTitleSubtitleToLandingContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlansSubtitleEn",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlansSubtitleEs",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlansTitleEn",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PlansTitleEs",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlansSubtitleEn",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "PlansSubtitleEs",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "PlansTitleEn",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "PlansTitleEs",
                table: "LandingContents");
        }
    }
}
