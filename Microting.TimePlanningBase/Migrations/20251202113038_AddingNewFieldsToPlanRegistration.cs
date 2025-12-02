using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingNewFieldsToPlanRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AbsenceHours",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EffectiveNetHours",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstWorkStartUtc",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HolidayHours",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSaturday",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSunday",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWorkEndUtc",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NightHours",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NormalHours",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OvertimeHours",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RuleEngineCalculated",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RuleEngineCalculatedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WeekendHours",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AbsenceHours",
                table: "PlanRegistrations",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EffectiveNetHours",
                table: "PlanRegistrations",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstWorkStartUtc",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HolidayHours",
                table: "PlanRegistrations",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSaturday",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSunday",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWorkEndUtc",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NightHours",
                table: "PlanRegistrations",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NormalHours",
                table: "PlanRegistrations",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OvertimeHours",
                table: "PlanRegistrations",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RuleEngineCalculated",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RuleEngineCalculatedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WeekendHours",
                table: "PlanRegistrations",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsenceHours",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "EffectiveNetHours",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "FirstWorkStartUtc",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "HolidayHours",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "IsSaturday",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "IsSunday",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "LastWorkEndUtc",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "NightHours",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "NormalHours",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "OvertimeHours",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "RuleEngineCalculated",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "RuleEngineCalculatedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "WeekendHours",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "AbsenceHours",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "EffectiveNetHours",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "FirstWorkStartUtc",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "HolidayHours",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "IsSaturday",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "IsSunday",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "LastWorkEndUtc",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "NightHours",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "NormalHours",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "OvertimeHours",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "RuleEngineCalculated",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "RuleEngineCalculatedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "WeekendHours",
                table: "PlanRegistrations");
        }
    }
}
