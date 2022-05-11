using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    public partial class AddingSumFlexStartEnd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SumFlex",
                table: "PlanRegistrationVersions",
                newName: "SumFlexEnd");

            migrationBuilder.RenameColumn(
                name: "SumFlex",
                table: "PlanRegistrations",
                newName: "SumFlexEnd");

            migrationBuilder.AddColumn<double>(
                name: "SumFlexStart",
                table: "PlanRegistrationVersions",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SumFlexStart",
                table: "PlanRegistrations",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SumFlexStart",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "SumFlexStart",
                table: "PlanRegistrations");

            migrationBuilder.RenameColumn(
                name: "SumFlexEnd",
                table: "PlanRegistrationVersions",
                newName: "SumFlex");

            migrationBuilder.RenameColumn(
                name: "SumFlexEnd",
                table: "PlanRegistrations",
                newName: "SumFlex");
        }
    }
}
