using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    public partial class AddingCaseMicrotingUid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaseMicrotingUid",
                table: "AssignedSiteVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CaseMicrotingUid",
                table: "AssignedSites",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseMicrotingUid",
                table: "AssignedSiteVersions");

            migrationBuilder.DropColumn(
                name: "CaseMicrotingUid",
                table: "AssignedSites");
        }
    }
}
