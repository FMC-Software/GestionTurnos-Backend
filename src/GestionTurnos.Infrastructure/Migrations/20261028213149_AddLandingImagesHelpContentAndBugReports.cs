using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLandingImagesHelpContentAndBugReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrlBarberia",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlEstetica",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlMasajes",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlPeluqueria",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlSpa",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlTattoo",
                table: "LandingContents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BugReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StepsToReproduce = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BugReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Faqs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    QuestionEs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerEs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faqs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HelpContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WhatsappNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupportEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleTextEs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleTextEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpContents", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BugReports");

            migrationBuilder.DropTable(
                name: "Faqs");

            migrationBuilder.DropTable(
                name: "HelpContents");

            migrationBuilder.DropColumn(
                name: "ImageUrlBarberia",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "ImageUrlEstetica",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "ImageUrlMasajes",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "ImageUrlPeluqueria",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "ImageUrlSpa",
                table: "LandingContents");

            migrationBuilder.DropColumn(
                name: "ImageUrlTattoo",
                table: "LandingContents");
        }
    }
}
