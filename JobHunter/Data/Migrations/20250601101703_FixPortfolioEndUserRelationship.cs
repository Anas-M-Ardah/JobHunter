using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunter.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPortfolioEndUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing foreign keys first
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Portfolios_PortfolioId",
                table: "Services");

            // Clean up orphaned Services (those without valid Portfolio references)
            migrationBuilder.Sql(@"
                DELETE FROM Services 
                WHERE PortfolioId IS NULL 
                OR PortfolioId NOT IN (SELECT PortfolioId FROM Portfolios)
            ");

            // Fix Services.PortfolioId to be non-nullable
            migrationBuilder.AlterColumn<Guid>(
                name: "PortfolioId",
                table: "Services",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // Handle Portfolio.EndUserId conversion from Guid to string
            // First, add a temporary column
            migrationBuilder.AddColumn<string>(
                name: "EndUserId_Temp",
                table: "Portfolios",
                type: "nvarchar(450)",
                nullable: true);

            // Convert existing Guid EndUserId to string format
            // This assumes your User.Id values can be matched somehow
            // You might need to adjust this based on your actual data
            migrationBuilder.Sql(@"
                UPDATE Portfolios 
                SET EndUserId_Temp = (
                    SELECT TOP 1 Id 
                    FROM AspNetUsers 
                    ORDER BY Id  -- You might want to add better logic here
                )
                WHERE EndUserId_Temp IS NULL
            ");

            // Drop the old EndUserId column
            migrationBuilder.DropColumn(
                name: "EndUserId",
                table: "Portfolios");

            // Rename the temporary column
            migrationBuilder.RenameColumn(
                name: "EndUserId_Temp",
                table: "Portfolios",
                newName: "EndUserId");

            // Make EndUserId non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "EndUserId",
                table: "Portfolios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            // Add index for Portfolio.EndUserId
            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_EndUserId",
                table: "Portfolios",
                column: "EndUserId");

            // Add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_AspNetUsers_EndUserId",
                table: "Portfolios",
                column: "EndUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Portfolios_PortfolioId",
                table: "Services",
                column: "PortfolioId",
                principalTable: "Portfolios",
                principalColumn: "PortfolioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_AspNetUsers_EndUserId",
                table: "Portfolios");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Portfolios_PortfolioId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_EndUserId",
                table: "Portfolios");

            migrationBuilder.AlterColumn<Guid>(
                name: "PortfolioId",
                table: "Services",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "EndUserId",
                table: "Portfolios",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Portfolios_PortfolioId",
                table: "Services",
                column: "PortfolioId",
                principalTable: "Portfolios",
                principalColumn: "PortfolioId");
        }
    }
}