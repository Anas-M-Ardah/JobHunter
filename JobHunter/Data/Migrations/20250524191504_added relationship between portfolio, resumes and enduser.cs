using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunter.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedrelationshipbetweenportfolioresumesandenduser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndUser",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "EndUser",
                table: "Portfolios");

            migrationBuilder.AddColumn<string>(
                name: "EndUserId",
                table: "Resumes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndUserId",
                table: "Portfolios",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_EndUserId",
                table: "Resumes",
                column: "EndUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_EndUserId",
                table: "Portfolios",
                column: "EndUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_AspNetUsers_EndUserId",
                table: "Portfolios",
                column: "EndUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_AspNetUsers_EndUserId",
                table: "Resumes",
                column: "EndUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_AspNetUsers_EndUserId",
                table: "Portfolios");

            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_AspNetUsers_EndUserId",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_EndUserId",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_EndUserId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "EndUserId",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "EndUserId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "EndUser",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndUser",
                table: "Portfolios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
