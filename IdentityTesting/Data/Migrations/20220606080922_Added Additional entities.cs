using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityTesting.Data.Migrations
{
    public partial class AddedAdditionalentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CourseID",
                table: "AspNetUsers",
                column: "CourseID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Course_CourseID",
                table: "AspNetUsers",
                column: "CourseID",
                principalTable: "Course",
                principalColumn: "CourseID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Course_CourseID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CourseID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CourseID",
                table: "AspNetUsers");
        }
    }
}
