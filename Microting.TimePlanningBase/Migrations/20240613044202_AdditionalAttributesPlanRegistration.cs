using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalAttributesPlanRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Pause1StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause1StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause2StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause2StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start1StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start2StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop1StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop2StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause1StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause1StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause2StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause2StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start1StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start2StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop1StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Stop2StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pause1StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause1StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause2StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause2StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Start1StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Start2StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Stop1StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Stop2StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause1StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause1StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause2StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause2StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Start1StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Start2StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Stop1StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Stop2StoppedAt",
                table: "PlanRegistrations");
        }
    }
}
