using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunter.Data.Migrations
{
    /// <inheritdoc />
    public partial class resumeEntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserInputCertificates",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserInputEducation",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserInputExperiences",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserInputLanguages",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserInputSkills",
                table: "Resumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Resumes_ResumeId",
                table: "Experiences");

            migrationBuilder.DropForeignKey(
                name: "FK_Languages_Resumes_ResumeId",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "UserInputCertificates",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "UserInputEducation",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "UserInputExperiences",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "UserInputLanguages",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "UserInputSkills",
                table: "Resumes");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Languages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Experiences",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Resumes_ResumeId",
                table: "Experiences",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "ResumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_Resumes_ResumeId",
                table: "Languages",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "ResumeId");
        }
    }
}
