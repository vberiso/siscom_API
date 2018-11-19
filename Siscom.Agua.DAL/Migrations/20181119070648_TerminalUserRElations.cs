using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class TerminalUserRElations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terminal_User_Terminal_id_terminal",
                table: "Terminal_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Terminal_User_AspNetUsers_id_user",
                table: "Terminal_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Terminal_User_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Terminal_User_id",
                table: "Terminal_User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Terminal_User",
                table: "Terminal_User");

            migrationBuilder.DropColumn(
                name: "TerminalUserUserId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "id_terminal",
                table: "Terminal_User");

            migrationBuilder.RenameColumn(
                name: "TerminalUserTermianlId",
                table: "Transaction",
                newName: "TerminalUserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Terminal_User",
                newName: "id_terminal_user");

            migrationBuilder.RenameColumn(
                name: "id_user",
                table: "Terminal_User",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Terminal_User_id_user",
                table: "Terminal_User",
                newName: "IX_Terminal_User_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Terminal_User",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "TerminalId",
                table: "Terminal_User",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Terminal_User",
                table: "Terminal_User",
                column: "id_terminal_user");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TerminalUserId",
                table: "Transaction",
                column: "TerminalUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_User_TerminalId",
                table: "Terminal_User",
                column: "TerminalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Terminal_User_Terminal_TerminalId",
                table: "Terminal_User",
                column: "TerminalId",
                principalTable: "Terminal",
                principalColumn: "id_terminal",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Terminal_User_AspNetUsers_UserId",
                table: "Terminal_User",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Terminal_User_TerminalUserId",
                table: "Transaction",
                column: "TerminalUserId",
                principalTable: "Terminal_User",
                principalColumn: "id_terminal_user",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terminal_User_Terminal_TerminalId",
                table: "Terminal_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Terminal_User_AspNetUsers_UserId",
                table: "Terminal_User");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Terminal_User_TerminalUserId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_TerminalUserId",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Terminal_User",
                table: "Terminal_User");

            migrationBuilder.DropIndex(
                name: "IX_Terminal_User_TerminalId",
                table: "Terminal_User");

            migrationBuilder.DropColumn(
                name: "TerminalId",
                table: "Terminal_User");

            migrationBuilder.RenameColumn(
                name: "TerminalUserId",
                table: "Transaction",
                newName: "TerminalUserTermianlId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Terminal_User",
                newName: "id_user");

            migrationBuilder.RenameColumn(
                name: "id_terminal_user",
                table: "Terminal_User",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Terminal_User_UserId",
                table: "Terminal_User",
                newName: "IX_Terminal_User_id_user");

            migrationBuilder.AddColumn<string>(
                name: "TerminalUserUserId",
                table: "Transaction",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "id_user",
                table: "Terminal_User",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_terminal",
                table: "Terminal_User",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Terminal_User_id",
                table: "Terminal_User",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Terminal_User",
                table: "Terminal_User",
                columns: new[] { "id_terminal", "id_user" });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction",
                columns: new[] { "TerminalUserTermianlId", "TerminalUserUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Terminal_User_Terminal_id_terminal",
                table: "Terminal_User",
                column: "id_terminal",
                principalTable: "Terminal",
                principalColumn: "id_terminal",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Terminal_User_AspNetUsers_id_user",
                table: "Terminal_User",
                column: "id_user",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Terminal_User_TerminalUserTermianlId_TerminalUserUserId",
                table: "Transaction",
                columns: new[] { "TerminalUserTermianlId", "TerminalUserUserId" },
                principalTable: "Terminal_User",
                principalColumns: new[] { "id_terminal", "id_user" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
