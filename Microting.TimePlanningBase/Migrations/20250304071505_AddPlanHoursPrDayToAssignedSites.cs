using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanHoursPrDayToAssignedSites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FridayPlanHours",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayPlanHours",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayPlanHours",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayPlanHours",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayPlanHours",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayPlanHours",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WednesdayPlanHours",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FridayPlanHours",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MondayPlanHours",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaturdayPlanHours",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SundayPlanHours",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThursdayPlanHours",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TuesdayPlanHours",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WednesdayPlanHours",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FridayPlanHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "MondayPlanHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SaturdayPlanHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "SundayPlanHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "ThursdayPlanHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "TuesdayPlanHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "WednesdayPlanHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "FridayPlanHours",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "MondayPlanHours",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SaturdayPlanHours",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "SundayPlanHours",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "ThursdayPlanHours",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "TuesdayPlanHours",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "WednesdayPlanHours",
                table: "AssignedSites");
        }
    }
}
