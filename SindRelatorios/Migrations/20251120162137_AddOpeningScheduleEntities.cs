using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SindRelatorios.Migrations
{
    /// <inheritdoc />
    public partial class AddOpeningScheduleEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstructorRestrictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorRestrictions_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpeningCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Region = table.Column<int>(type: "integer", nullable: false),
                    IsExtra = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningCalendars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpeningSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OpeningCalendarId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Shift = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Observation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningSlots_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpeningSlots_OpeningCalendars_OpeningCalendarId",
                        column: x => x.OpeningCalendarId,
                        principalTable: "OpeningCalendars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorRestrictions_InstructorId",
                table: "InstructorRestrictions",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningSlots_InstructorId",
                table: "OpeningSlots",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningSlots_OpeningCalendarId",
                table: "OpeningSlots",
                column: "OpeningCalendarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorRestrictions");

            migrationBuilder.DropTable(
                name: "OpeningSlots");

            migrationBuilder.DropTable(
                name: "OpeningCalendars");
        }
    }
}
