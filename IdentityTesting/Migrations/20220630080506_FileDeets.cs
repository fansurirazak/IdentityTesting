using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityTesting.Migrations
{
    public partial class FileDeets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ProjectProp",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ProjectProp",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ProjectProp");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ProjectProp");
        }
    }
}
