using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TerminalUserKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TerminalUserTermianlId",
                table: "Transaction",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TerminalUserUserId",
                table: "Transaction",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Terminal_User",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    open_date = table.Column<DateTime>(type: "date", nullable: false),
                    in_operation = table.Column<bool>(nullable: false, defaultValue: false),
                    id_terminal = table.Column<int>(nullable: false),
                    id_user = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminal_User", x => new { x.id_terminal, x.id_user });
                    table.UniqueConstraint("AK_Terminal_User_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_Terminal_User_Terminal_id_terminal",
                        column: x => x.id_terminal,
                        principalTable: "Terminal",
                        principalColumn: "id_terminal",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Terminal_User_AspNetUsers_id_user",
                        column: x => x.id_user,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction",
                columns: new[] { "TerminalUserTermianlId", "TerminalUserUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_User_id_user",
                table: "Terminal_User",
                column: "id_user");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Terminal_User_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction",
                columns: new[] { "TerminalUserTermianlId", "TerminalUserUserId" },
                principalTable: "Terminal_User",
                principalColumns: new[] { "id_terminal", "id_user" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Terminal_User_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction");

            migrationBuilder.DropTable(
                name: "Terminal_User");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TerminalUserTermianlId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TerminalUserUserId",
                table: "Transaction");
        }
    }
}
