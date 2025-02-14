using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingBetterOptionsForPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDoubleShift",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift1",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift2",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift1",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift2",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift1",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift2",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDoubleShift",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift1",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedBreakOfShift2",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift1",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedEndOfShift2",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift1",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedStartOfShift2",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDoubleShift",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift1",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift2",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift1",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift2",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift1",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift2",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "IsDoubleShift",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift1",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedBreakOfShift2",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift1",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedEndOfShift2",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift1",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlannedStartOfShift2",
                table: "PlanRegistrations");
        }
    }
}
