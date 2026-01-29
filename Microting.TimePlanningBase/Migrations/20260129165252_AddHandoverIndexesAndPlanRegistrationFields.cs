using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddHandoverIndexesAndPlanRegistrationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ContentHandedOverAtUtc",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentHandoverFromSdkSitId",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentHandoverRequestId",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentHandoverToSdkSitId",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ContentHandedOverAtUtc",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentHandoverFromSdkSitId",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentHandoverRequestId",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContentHandoverToSdkSitId",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanRegistrationContentHandoverRequests_FromSdkSitId_Status_~",
                table: "PlanRegistrationContentHandoverRequests",
                columns: new[] { "FromSdkSitId", "Status", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanRegistrationContentHandoverRequests_ToSdkSitId_Status_Da~",
                table: "PlanRegistrationContentHandoverRequests",
                columns: new[] { "ToSdkSitId", "Status", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlanRegistrationContentHandoverRequests_FromSdkSitId_Status_~",
                table: "PlanRegistrationContentHandoverRequests");

            migrationBuilder.DropIndex(
                name: "IX_PlanRegistrationContentHandoverRequests_ToSdkSitId_Status_Da~",
                table: "PlanRegistrationContentHandoverRequests");

            migrationBuilder.DropColumn(
                name: "ContentHandedOverAtUtc",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "ContentHandoverFromSdkSitId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "ContentHandoverRequestId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "ContentHandoverToSdkSitId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "ContentHandedOverAtUtc",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "ContentHandoverFromSdkSitId",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "ContentHandoverRequestId",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "ContentHandoverToSdkSitId",
                table: "PlanRegistrations");
        }
    }
}
