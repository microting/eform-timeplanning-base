using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microting.TimePlanningBase.Migrations
{
    public partial class AddingTranslationsToMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DaName",
                table: "Messages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DeName",
                table: "Messages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EnName",
                table: "Messages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Name", "DaName", "DeName", "EnName" },
                values: new object[,]
                {
                    { 1, "Day Off", "Fridag", "Freier Tag", "Day off" },
                    { 2, "Vacation", "Ferie", "Urlaub", "Vacation" },
                    { 3, "Sick", "Syg", "Krank", "Sick" },
                    { 4, "Course", "Kursus", "Kurs", "Course" },
                    { 5, "Leave of absence", "Orlov", "Urlaub", "Leave of absence" },
                    { 6, "Care", "", "", "" },
                    { 7, "Children's 1st sick", "Barns 1. sygedag", "1. Krankheitstag der Kinder", "Children's 1st sick" },
                    { 8, "Children's 2nd sick", "Barns 2. sygedag", "2. Krankheitstag der Kinder", "Children's 2nd sick" },
                    { 9, "Time off", "", "", "" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaName",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DeName",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EnName",
                table: "Messages");
        }
    }
}