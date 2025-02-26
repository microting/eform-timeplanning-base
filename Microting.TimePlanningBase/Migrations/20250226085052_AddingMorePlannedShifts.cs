using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingMorePlannedShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift3",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift4",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift5",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift3",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift4",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift5",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift3",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift4",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift5",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift3",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift4",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift5",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift3",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift4",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift5",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift3",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift4",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift5",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift3",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift4",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift5",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift3",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift4",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift5",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift3",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift4",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift5",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift3",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift4",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift5",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift3",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift4",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift5",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift3",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift4",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift5",
                table: "PlanRegistrations");
        }
    }
}
