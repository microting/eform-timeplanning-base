using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddingTenPausesExtraPrShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Pause100StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause100StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause101StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause101StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause102StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause102StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause10StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause10StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause11StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause11StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause12StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause12StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause13StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause13StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause14StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause14StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause15StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause15StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause16StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause16StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause17StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause17StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause18StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause18StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause19StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause19StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause200StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause200StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause201StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause201StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause202StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause202StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause20StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause20StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause21StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause21StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause22StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause22StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause23StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause23StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause24StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause24StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause25StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause25StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause26StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause26StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause27StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause27StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause28StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause28StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause29StartedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause29StoppedAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause100StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause100StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause101StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause101StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause102StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause102StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause10StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause10StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause11StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause11StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause12StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause12StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause13StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause13StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause14StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause14StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause15StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause15StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause16StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause16StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause17StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause17StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause18StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause18StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause19StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause19StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause200StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause200StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause201StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause201StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause202StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause202StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause20StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause20StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause21StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause21StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause22StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause22StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause23StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause23StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause24StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause24StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause25StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause25StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause26StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause26StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause27StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause27StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause28StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause28StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause29StartedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Pause29StoppedAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pause100StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause100StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause101StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause101StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause102StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause102StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause10StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause10StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause11StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause11StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause12StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause12StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause13StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause13StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause14StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause14StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause15StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause15StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause16StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause16StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause17StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause17StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause18StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause18StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause19StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause19StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause200StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause200StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause201StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause201StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause202StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause202StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause20StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause20StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause21StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause21StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause22StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause22StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause23StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause23StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause24StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause24StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause25StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause25StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause26StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause26StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause27StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause27StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause28StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause28StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause29StartedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause29StoppedAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Pause100StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause100StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause101StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause101StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause102StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause102StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause10StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause10StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause11StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause11StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause12StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause12StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause13StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause13StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause14StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause14StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause15StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause15StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause16StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause16StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause17StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause17StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause18StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause18StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause19StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause19StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause200StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause200StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause201StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause201StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause202StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause202StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause20StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause20StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause21StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause21StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause22StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause22StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause23StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause23StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause24StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause24StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause25StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause25StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause26StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause26StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause27StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause27StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause28StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause28StoppedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause29StartedAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "Pause29StoppedAt",
                table: "PlanRegistrations");
        }
    }
}
