using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunter.Data.Migrations
{
    /// <inheritdoc />
    public partial class replacedEndUserWithEndUserId2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EndUserId",
                table: "Resumes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_EndUserId",
                table: "Resumes",
                column: "EndUserId");

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
                name: "FK_Resumes_AspNetUsers_EndUserId",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_EndUserId",
                table: "Resumes");

            migrationBuilder.AlterColumn<Guid>(
                name: "EndUserId",
                table: "Resumes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
