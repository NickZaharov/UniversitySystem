using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UniversitySystem.Migrations
{
    public partial class MigrateDB7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseMembers",
                table: "CourseMembers");

            migrationBuilder.AddColumn<int>(
                name: "CourseMemberId",
                table: "CourseMembers",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseMembers",
                table: "CourseMembers",
                column: "CourseMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMembers_CourseId",
                table: "CourseMembers",
                column: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseMembers",
                table: "CourseMembers");

            migrationBuilder.DropIndex(
                name: "IX_CourseMembers_CourseId",
                table: "CourseMembers");

            migrationBuilder.DropColumn(
                name: "CourseMemberId",
                table: "CourseMembers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseMembers",
                table: "CourseMembers",
                columns: new[] { "CourseId", "StudentId" });
        }
    }
}
