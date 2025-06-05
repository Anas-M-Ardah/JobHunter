using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunter.Data.Migrations
{
    /// <inheritdoc />
    public partial class addCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificate_Resumes_ResumeId",
                table: "Certificate");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Resumes_ResumeId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Resumes_ResumeId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certificate",
                table: "Certificate");

            migrationBuilder.RenameTable(
                name: "Certificate",
                newName: "Certificates");

            migrationBuilder.RenameIndex(
                name: "IX_Certificate_ResumeId",
                table: "Certificates",
                newName: "IX_Certificates_ResumeId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Skills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Educations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Certificates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certificates",
                table: "Certificates",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Resumes_ResumeId",
                table: "Certificates",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "ResumeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Resumes_ResumeId",
                table: "Educations",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "ResumeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Resumes_ResumeId",
                table: "Skills",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "ResumeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Resumes_ResumeId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Educations_Resumes_ResumeId",
                table: "Educations");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Resumes_ResumeId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Certificates",
                table: "Certificates");

            migrationBuilder.RenameTable(
                name: "Certificates",
                newName: "Certificate");

            migrationBuilder.RenameIndex(
                name: "IX_Certificates_ResumeId",
                table: "Certificate",
                newName: "IX_Certificate_ResumeId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Skills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Educations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Certificate",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Certificate",
                table: "Certificate",
                column: "CertificateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificate_Resumes_ResumeId",
                table: "Certificate",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "ResumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Educations_Resumes_ResumeId",
                table: "Educations",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "ResumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Resumes_ResumeId",
                table: "Skills",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "ResumeId");
        }
    }
}
