using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingMoreAttributesToAssignedSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoBreakCalculationActive",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FridayBreakMinutesDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FridayBreakMinutesPrDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FridayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayBreakMinutesDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayBreakMinutesPrDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayBreakMinutesDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayBreakMinutesPrDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayBreakMinutesDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayBreakMinutesPrDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayBreakMinutesDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayBreakMinutesPrDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayBreakMinutesDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayBreakMinutesPrDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WednesdayBreakMinutesDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WednesdayBreakMinutesPrDivider",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WednesdayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AutoBreakCalculationActive",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FridayBreakMinutesDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FridayBreakMinutesPrDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FridayBreakMinutesUpperLimit",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayBreakMinutesDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayBreakMinutesPrDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayBreakMinutesUpperLimit",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayBreakMinutesDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayBreakMinutesPrDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayBreakMinutesUpperLimit",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayBreakMinutesDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayBreakMinutesPrDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayBreakMinutesUpperLimit",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayBreakMinutesDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayBreakMinutesPrDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayBreakMinutesUpperLimit",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayBreakMinutesDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayBreakMinutesPrDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayBreakMinutesUpperLimit",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WednesdayBreakMinutesDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WednesdayBreakMinutesPrDivider",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WednesdayBreakMinutesUpperLimit",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoBreakCalculationActive",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "FridayBreakMinutesDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "FridayBreakMinutesPrDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "FridayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "MondayBreakMinutesDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "MondayBreakMinutesPrDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "MondayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SaturdayBreakMinutesDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SaturdayBreakMinutesPrDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SaturdayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SundayBreakMinutesDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SundayBreakMinutesPrDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SundayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "ThursdayBreakMinutesDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "ThursdayBreakMinutesPrDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "ThursdayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "TuesdayBreakMinutesDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "TuesdayBreakMinutesPrDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "TuesdayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "WednesdayBreakMinutesDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "WednesdayBreakMinutesPrDivider",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "WednesdayBreakMinutesUpperLimit",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "AutoBreakCalculationActive",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "FridayBreakMinutesDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "FridayBreakMinutesPrDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "FridayBreakMinutesUpperLimit",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "MondayBreakMinutesDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "MondayBreakMinutesPrDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "MondayBreakMinutesUpperLimit",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SaturdayBreakMinutesDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SaturdayBreakMinutesPrDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SaturdayBreakMinutesUpperLimit",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SundayBreakMinutesDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SundayBreakMinutesPrDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SundayBreakMinutesUpperLimit",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "ThursdayBreakMinutesDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "ThursdayBreakMinutesPrDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "ThursdayBreakMinutesUpperLimit",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "TuesdayBreakMinutesDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "TuesdayBreakMinutesPrDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "TuesdayBreakMinutesUpperLimit",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "WednesdayBreakMinutesDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "WednesdayBreakMinutesPrDivider",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "WednesdayBreakMinutesUpperLimit",
                table: "AssignedSites");
        }
    }
}
