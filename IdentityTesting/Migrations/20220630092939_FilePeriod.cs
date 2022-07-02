using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityTesting.Migrations
{
    public partial class FilePeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjSemester",
                table: "ProjectProp",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjSession",
                table: "ProjectProp",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjSemester",
                table: "ProjectProp");

            migrationBuilder.DropColumn(
                name: "ProjSession",
                table: "ProjectProp");
        }
    }
}
