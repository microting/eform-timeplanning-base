using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    /// <inheritdoc />
    public partial class AddAbsenceRequestEntitiesAndPlanRegistrationAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AbsenceApprovedAtUtc",
                table: "PlanRegistrationVersions",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbsenceApprovedBySdkSitId",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbsenceMessageId",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbsenceRequestId",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EffectiveNetHoursInSeconds",
                table: "PlanRegistrationVersions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AbsenceApprovedAtUtc",
                table: "PlanRegistrations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbsenceApprovedBySdkSitId",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbsenceMessageId",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbsenceRequestId",
                table: "PlanRegistrations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AbsenceRequestDayVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AbsenceRequestDayId = table.Column<int>(type: "int", nullable: false),
                    AbsenceRequestId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AbsenceRequestDayVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AbsenceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RequestedBySdkSitId = table.Column<int>(type: "int", nullable: false),
                    DecidedBySdkSitId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateTo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RequestComment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DecisionComment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DecidedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                    table.PrimaryKey("PK_AbsenceRequests", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AbsenceRequestVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AbsenceRequestId = table.Column<int>(type: "int", nullable: false),
                    RequestedBySdkSitId = table.Column<int>(type: "int", nullable: false),
                    DecidedBySdkSitId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateTo = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    RequestComment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DecisionComment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DecidedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                    table.PrimaryKey("PK_AbsenceRequestVersions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AbsenceRequestDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AbsenceRequestId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AbsenceRequestDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbsenceRequestDays_AbsenceRequests_AbsenceRequestId",
                        column: x => x.AbsenceRequestId,
                        principalTable: "AbsenceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbsenceRequestDays_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceRequestDays_AbsenceRequestId",
                table: "AbsenceRequestDays",
                column: "AbsenceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AbsenceRequestDays_MessageId",
                table: "AbsenceRequestDays",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbsenceRequestDays");

            migrationBuilder.DropTable(
                name: "AbsenceRequestDayVersions");

            migrationBuilder.DropTable(
                name: "AbsenceRequestVersions");

            migrationBuilder.DropTable(
                name: "AbsenceRequests");

            migrationBuilder.DropColumn(
                name: "AbsenceApprovedAtUtc",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "AbsenceApprovedBySdkSitId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "AbsenceMessageId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "AbsenceRequestId",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "EffectiveNetHoursInSeconds",
                table: "PlanRegistrationVersions");

            migrationBuilder.DropColumn(
                name: "AbsenceApprovedAtUtc",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "AbsenceApprovedBySdkSitId",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "AbsenceMessageId",
                table: "PlanRegistrations");

            migrationBuilder.DropColumn(
                name: "AbsenceRequestId",
                table: "PlanRegistrations");
        }
    }
}
