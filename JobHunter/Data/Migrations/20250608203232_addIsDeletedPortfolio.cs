using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobHunter.Data.Migrations
{
    /// <inheritdoc />
    public partial class addIsDeletedPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Portfolios",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Portfolios");
        }
    }
}
