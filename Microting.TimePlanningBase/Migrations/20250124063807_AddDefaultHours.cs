using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BreakFriday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday",
                table: "AssignedSites",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakFriday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakMonday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSaturday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSunday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakThursday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakTuesday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakWednesday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndFriday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndMonday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSaturday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSunday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndThursday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndTuesday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndWednesday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartFriday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartMonday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSaturday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSunday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartThursday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartTuesday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartWednesday",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakFriday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakMonday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSaturday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSunday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakThursday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakTuesday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakWednesday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndFriday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndMonday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSaturday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSunday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndThursday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndTuesday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndWednesday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartFriday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartMonday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSaturday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSunday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartThursday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartTuesday",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartWednesday",
                table: "AssignedSites");
        }
    }
}
