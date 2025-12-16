using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingTimeRuleSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkingTimeRuleSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WeeklyNormalHours = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    DailyNormalHours = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    MinimumDailyRest = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    MinimumWeeklyRest = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    WeekStartsOn = table.Column<int>(type: "int", nullable: false),
                    NightStart = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    NightEnd = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    OvertimeBasis = table.Column<int>(type: "int", nullable: false),
                    UnpaidBreakPerDay = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    PaidBreakPerDay = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    RuleSetName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RuleSetVersion = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingTimeRuleSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "WorkingTimeRuleSettingsVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkingTimeRuleSettingsId = table.Column<int>(type: "int", nullable: false),
                    WeeklyNormalHours = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    DailyNormalHours = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    MinimumDailyRest = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    MinimumWeeklyRest = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    WeekStartsOn = table.Column<int>(type: "int", nullable: false),
                    NightStart = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    NightEnd = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    OvertimeBasis = table.Column<int>(type: "int", nullable: false),
                    UnpaidBreakPerDay = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    PaidBreakPerDay = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    RuleSetName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RuleSetVersion = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    WorkflowState = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingTimeRuleSettingsVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkingTimeRuleSettings");

            migrationBuilder.DropTable(
                name: "WorkingTimeRuleSettingsVersions");
        }
    }
}
