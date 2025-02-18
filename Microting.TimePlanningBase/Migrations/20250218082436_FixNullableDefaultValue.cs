using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class FixNullableDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // update assignedSites have the nullable fields set to null
            migrationBuilder.Sql("update AssignedSites set StartMonday = null;");
            migrationBuilder.Sql("update AssignedSites set EndMonday = null;");
            migrationBuilder.Sql("update AssignedSites set BreakMonday = null;");
            migrationBuilder.Sql("update AssignedSites set StartTuesday = null;");
            migrationBuilder.Sql("update AssignedSites set EndTuesday = null;");
            migrationBuilder.Sql("update AssignedSites set BreakTuesday = null;");
            migrationBuilder.Sql("update AssignedSites set StartWednesday = null;");
            migrationBuilder.Sql("update AssignedSites set EndWednesday = null;");
            migrationBuilder.Sql("update AssignedSites set BreakWednesday = null;");
            migrationBuilder.Sql("update AssignedSites set StartThursday = null;");
            migrationBuilder.Sql("update AssignedSites set EndThursday = null;");
            migrationBuilder.Sql("update AssignedSites set BreakThursday = null;");
            migrationBuilder.Sql("update AssignedSites set StartFriday = null;");
            migrationBuilder.Sql("update AssignedSites set EndFriday = null;");
            migrationBuilder.Sql("update AssignedSites set BreakFriday = null;");
            migrationBuilder.Sql("update AssignedSites set StartSaturday = null;");
            migrationBuilder.Sql("update AssignedSites set EndSaturday = null;");
            migrationBuilder.Sql("update AssignedSites set BreakSaturday = null;");
            migrationBuilder.Sql("update AssignedSites set StartSunday = null;");
            migrationBuilder.Sql("update AssignedSites set EndSunday = null;");
            migrationBuilder.Sql("update AssignedSites set BreakSunday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set StartMonday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set EndMonday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set BreakMonday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set StartTuesday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set EndTuesday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set BreakTuesday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set StartWednesday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set EndWednesday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set BreakWednesday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set StartThursday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set EndThursday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set BreakThursday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set StartFriday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set EndFriday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set BreakFriday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set StartSaturday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set EndSaturday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set BreakSaturday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set StartSunday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set EndSunday = null;");
            migrationBuilder.Sql("update AssignedSiteVersions set BreakSunday = null;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
