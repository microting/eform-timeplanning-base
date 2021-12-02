using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    public partial class FixingPlanRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanRegistrations_AssignedSites_AssignedSiteId",
                table: "PlanRegistrations");

            migrationBuilder.RenameColumn(
                name: "AssignedSiteId",
                table: "PlanRegistrationVersions",
                newName: "SdkSitId");

            migrationBuilder.RenameColumn(
                name: "AssignedSiteId",
                table: "PlanRegistrations",
                newName: "SdkSitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanRegistrations_AssignedSites_AssignedSiteId",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "SdkSitId",
                table: "PlanRegistrations");

            migrationBuilder.RenameColumn(
                name: "SdkSitId",
                table: "PlanRegistrationVersions",
                newName: "AssignedSiteId");

            migrationBuilder.AlterColumn<int>(
                name: "AssignedSiteId",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanRegistrations_AssignedSites_AssignedSiteId",
                table: "PlanRegistrations",
                column: "AssignedSiteId",
                principalTable: "AssignedSites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
