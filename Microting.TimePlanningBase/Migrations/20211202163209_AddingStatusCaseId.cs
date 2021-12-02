using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    public partial class AddingStatusCaseId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusCaseId",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusCaseId",
                table: "PlanRegistrations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCaseId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "StatusCaseId",
                table: "PlanRegistrations");
        }
    }
}
