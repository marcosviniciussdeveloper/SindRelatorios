using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindRelatorios.Migrations
{
    /// <inheritdoc />
    public partial class AddOpningType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "OpeningCalendars",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "OpeningCalendars");
        }
    }
}
