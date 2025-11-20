using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindRelatorios.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingIsExtraColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExtra",
                table: "OpeningSlots",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExtra",
                table: "OpeningSlots");
        }
    }
}
