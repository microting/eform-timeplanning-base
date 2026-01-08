using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingSecondsAttributesForBetterPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AbsenceHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlexInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HolidayHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NettoHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NettoHoursOverrideInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NightHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NormalHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OvertimeHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaiedOutFlexInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlanHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SumFlexEndInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SumFlexStartInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeekendHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbsenceHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EffectiveNetHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlexInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HolidayHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NettoHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NettoHoursOverrideInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NightHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NormalHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OvertimeHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaiedOutFlexInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlanHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SumFlexEndInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SumFlexStartInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeekendHoursInSeconds",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsenceHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "FlexInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "HolidayHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "NettoHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "NettoHoursOverrideInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "NightHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "NormalHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "OvertimeHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PaiedOutFlexInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "PlanHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "SumFlexEndInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "SumFlexStartInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "WeekendHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "AbsenceHoursInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "EffectiveNetHoursInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "FlexInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "HolidayHoursInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "NettoHoursInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "NettoHoursOverrideInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "NightHoursInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "NormalHoursInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "OvertimeHoursInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PaiedOutFlexInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "PlanHoursInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "SumFlexEndInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "SumFlexStartInSeconds",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "WeekendHoursInSeconds",
                table: "PlanRegistrations");
        }
    }
}
