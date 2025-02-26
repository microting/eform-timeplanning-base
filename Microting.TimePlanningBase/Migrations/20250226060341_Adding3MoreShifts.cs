using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class Adding3MoreShifts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pause3Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause3StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause3StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause4Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause4StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause4StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause5Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause5StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause5StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Start3Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start3StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Start4Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start4StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Start5Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start5StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stop3Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop3StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stop4Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop4StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stop5Id",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop5StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause3Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause3StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause3StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause4Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause4StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause4StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pause5Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause5StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause5StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Start3Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start3StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Start4Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start4StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Start5Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start5StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stop3Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop3StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stop4Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop4StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stop5Id",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop5StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowAcceptOfPlannedHours",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowEditOfRegistrations",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowPersonalTimeRegistration",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday2NdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday3RdShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday4ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday5ThShift",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseOneMinuteIntervals",
                table: "AssignedSiteVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowAcceptOfPlannedHours",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowEditOfRegistrations",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowPersonalTimeRegistration",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakFriday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMonday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSaturday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakSunday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakThursday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakTuesday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakWednesday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndFriday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndMonday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSaturday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndSunday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndThursday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTuesday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndWednesday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartFriday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartMonday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSaturday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartSunday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartThursday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTuesday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday2NdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday3RdShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday4ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartWednesday5ThShift",
                table: "AssignedSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseOneMinuteIntervals",
                table: "AssignedSites",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pause3Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause3StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause3StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause4Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause4StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause4StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause5Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause5StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause5StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Start3Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Start3StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Start4Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Start4StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Start5Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Start5StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Stop3Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Stop3StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Stop4Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Stop4StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Stop5Id",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Stop5StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause3Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause3StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause3StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause4Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause4StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause4StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause5Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause5StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause5StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Start3Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Start3StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Start4Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Start4StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Start5Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Start5StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Stop3Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Stop3StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Stop4Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Stop4StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Stop5Id",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Stop5StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "AllowAcceptOfPlannedHours",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "AllowEditOfRegistrations",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "AllowPersonalTimeRegistration",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakFriday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakFriday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakFriday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakFriday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakMonday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakMonday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakMonday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakMonday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSaturday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSaturday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSaturday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSaturday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSunday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSunday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSunday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakSunday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakThursday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakThursday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakThursday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakThursday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakTuesday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakTuesday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakTuesday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakTuesday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakWednesday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakWednesday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakWednesday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "BreakWednesday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndFriday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndFriday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndFriday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndFriday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndMonday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndMonday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndMonday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndMonday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSaturday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSaturday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSaturday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSaturday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSunday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSunday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSunday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndSunday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndThursday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndThursday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndThursday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndThursday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndTuesday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndTuesday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndTuesday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndTuesday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndWednesday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndWednesday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndWednesday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "EndWednesday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartFriday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartFriday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartFriday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartFriday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartMonday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartMonday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartMonday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartMonday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSaturday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSaturday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSaturday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSaturday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSunday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSunday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSunday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartSunday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartThursday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartThursday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartThursday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartThursday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartTuesday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartTuesday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartTuesday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartTuesday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartWednesday2NdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartWednesday3RdShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartWednesday4ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "StartWednesday5ThShift",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "UseOneMinuteIntervals",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "AllowAcceptOfPlannedHours",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "AllowEditOfRegistrations",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "AllowPersonalTimeRegistration",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakFriday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakFriday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakFriday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakFriday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakMonday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakMonday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakMonday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakMonday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSaturday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSaturday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSaturday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSaturday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSunday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSunday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSunday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakSunday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakThursday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakThursday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakThursday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakThursday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakTuesday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakTuesday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakTuesday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakTuesday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakWednesday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakWednesday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakWednesday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "BreakWednesday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndFriday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndFriday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndFriday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndFriday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndMonday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndMonday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndMonday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndMonday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSaturday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSaturday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSaturday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSaturday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSunday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSunday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSunday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndSunday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndThursday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndThursday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndThursday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndThursday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndTuesday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndTuesday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndTuesday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndTuesday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndWednesday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndWednesday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndWednesday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "EndWednesday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartFriday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartFriday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartFriday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartFriday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartMonday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartMonday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartMonday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartMonday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSaturday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSaturday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSaturday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSaturday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSunday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSunday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSunday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartSunday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartThursday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartThursday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartThursday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartThursday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartTuesday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartTuesday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartTuesday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartTuesday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartWednesday2NdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartWednesday3RdShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartWednesday4ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "StartWednesday5ThShift",
                table: "AssignedSites");

            migrationBuilder.DropColumn(
                name: "UseOneMinuteIntervals",
                table: "AssignedSites");
        }
    }
}
