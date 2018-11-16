using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class BranchOfficeHoursType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchOfficeId",
                table: "Terminal",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchOfficeId",
                table: "Folio",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Branch_Office",
                columns: table => new
                {
                    id_branch_office = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(maxLength: 30, nullable: false),
                    opening = table.Column<DateTime>(nullable: false),
                    closing = table.Column<DateTime>(nullable: false),
                    dont_close = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch_Office", x => x.id_branch_office);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_BranchOfficeId",
                table: "Terminal",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Folio_BranchOfficeId",
                table: "Folio",
                column: "BranchOfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Folio_Branch_Office_BranchOfficeId",
                table: "Folio",
                column: "BranchOfficeId",
                principalTable: "Branch_Office",
                principalColumn: "id_branch_office",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Terminal_Branch_Office_BranchOfficeId",
                table: "Terminal",
                column: "BranchOfficeId",
                principalTable: "Branch_Office",
                principalColumn: "id_branch_office",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folio_Branch_Office_BranchOfficeId",
                table: "Folio");

            migrationBuilder.DropForeignKey(
                name: "FK_Terminal_Branch_Office_BranchOfficeId",
                table: "Terminal");

            migrationBuilder.DropTable(
                name: "Branch_Office");

            migrationBuilder.DropIndex(
                name: "IX_Terminal_BranchOfficeId",
                table: "Terminal");

            migrationBuilder.DropIndex(
                name: "IX_Folio_BranchOfficeId",
                table: "Folio");

            migrationBuilder.DropColumn(
                name: "BranchOfficeId",
                table: "Terminal");

            migrationBuilder.DropColumn(
                name: "BranchOfficeId",
                table: "Folio");
        }
    }
}
