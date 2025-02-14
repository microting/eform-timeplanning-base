using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingMoreOptionsForPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AbsenceWithoutPermission",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnVacation",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OtherAllowedAbsence",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sick",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AbsenceWithoutPermission",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnVacation",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OtherAllowedAbsence",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sick",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Resigned",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Resigned",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsenceWithoutPermission",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "OnVacation",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "OtherAllowedAbsence",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Sick",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "AbsenceWithoutPermission",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "OnVacation",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "OtherAllowedAbsence",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Sick",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Resigned",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "Resigned",
                table: "AssignedSites");
        }
    }
}
