using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddActicatingOfShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FifthShiftActive",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FourthShiftActive",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ThirdShiftActive",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FifthShiftActive",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FourthShiftActive",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ThirdShiftActive",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FifthShiftActive",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "FourthShiftActive",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "ThirdShiftActive",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "FifthShiftActive",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "FourthShiftActive",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "ThirdShiftActive",
                table: "AssignedSites");
        }
    }
}
