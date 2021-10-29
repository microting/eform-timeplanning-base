using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microting.TimePlanningBase.Migrations
{
    public partial class AddedTableMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "PlanRegistrations",
                newName: "MessageId");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Day Off" },
                    { 2, "Vacation" },
                    { 3, "Sick" },
                    { 4, "Course" },
                    { 5, "Leave of absence" },
                    { 6, "Care" },
                    { 7, "Children's 1st sick" },
                    { 8, "Children's 2nd sick" },
                    { 9, "Time off" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanRegistrations_MessageId",
                table: "PlanRegistrations",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlanRegistrations_Messages_MessageId",
                table: "PlanRegistrations",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanRegistrations_Messages_MessageId",
                table: "PlanRegistrations");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_PlanRegistrations_MessageId",
                table: "PlanRegistrations");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "PlanRegistrations",
                newName: "Message");
        }
    }
}
