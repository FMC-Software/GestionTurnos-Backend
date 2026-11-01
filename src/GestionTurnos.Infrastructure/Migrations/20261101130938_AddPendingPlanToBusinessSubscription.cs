using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionTurnos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingPlanToBusinessSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PendingPlanId",
                table: "BusinessSubscriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessSubscriptions_PendingPlanId",
                table: "BusinessSubscriptions",
                column: "PendingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessSubscriptions_Plans_PendingPlanId",
                table: "BusinessSubscriptions",
                column: "PendingPlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessSubscriptions_Plans_PendingPlanId",
                table: "BusinessSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_BusinessSubscriptions_PendingPlanId",
                table: "BusinessSubscriptions");

            migrationBuilder.DropColumn(
                name: "PendingPlanId",
                table: "BusinessSubscriptions");
        }
    }
}
