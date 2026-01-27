using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddReconciledAndTransferredToPayrollToPlanRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Reconciled",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReconciledAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TransferredToPayroll",
                table: "PlanRegistrationVersions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransferredToPayrollAt",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Reconciled",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReconciledAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TransferredToPayroll",
                table: "PlanRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransferredToPayrollAt",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reconciled",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "ReconciledAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "TransferredToPayroll",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "TransferredToPayrollAt",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "Reconciled",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "ReconciledAt",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "TransferredToPayroll",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "TransferredToPayrollAt",
                table: "PlanRegistrations");
        }
    }
}
