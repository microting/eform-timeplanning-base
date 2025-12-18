using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToPlanRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Handle existing duplicates by keeping only the most recently updated record per (SdkSitId, Date, WorkflowState)
            // This SQL deletes duplicate records, keeping only the one with the highest Id
            // In this system, higher Id values correspond to more recent records as they are auto-incremented
            migrationBuilder.Sql(@"
                DELETE p1 FROM PlanRegistrations p1
                INNER JOIN (
                    SELECT SdkSitId, Date, WorkflowState, MAX(Id) as MaxId
                    FROM PlanRegistrations
                    GROUP BY SdkSitId, Date, WorkflowState
                    HAVING COUNT(*) > 1
                ) p2 ON p1.SdkSitId = p2.SdkSitId 
                    AND p1.Date = p2.Date 
                    AND p1.WorkflowState = p2.WorkflowState
                    AND p1.Id < p2.MaxId;
            ");

            // Step 2: Create unique index on (SdkSitId, Date, WorkflowState)
            migrationBuilder.CreateIndex(
                name: "IX_PlanRegistrations_SdkSitId_Date_WorkflowState",
                table: "PlanRegistrations",
                columns: new[] { "SdkSitId", "Date", "WorkflowState" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the unique index
            migrationBuilder.DropIndex(
                name: "IX_PlanRegistrations_SdkSitId_Date_WorkflowState",
                table: "PlanRegistrations");
        }
    }
}
