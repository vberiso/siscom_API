using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class BranchOfficeTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "open",
                table: "Branch_Office",
                newName: "opening");

            migrationBuilder.RenameColumn(
                name: "close",
                table: "Branch_Office",
                newName: "closing");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "opening",
                table: "Branch_Office",
                newName: "open");

            migrationBuilder.RenameColumn(
                name: "closing",
                table: "Branch_Office",
                newName: "close");
        }
    }
}
