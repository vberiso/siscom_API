using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AgreementFileSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 15, 13, 36, 22, 688, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 10, 12, 39, 59, 939, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Agreement_File",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "size",
                table: "Agreement_File",
                maxLength: 6,
                nullable: false,
                defaultValue: "0");

            migrationBuilder.AddColumn<DateTime>(
                name: "upload_date",
                table: "Agreement_File",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Agreement_File",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_File_UserId",
                table: "Agreement_File",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agreement_File_AspNetUsers_UserId",
                table: "Agreement_File",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agreement_File_AspNetUsers_UserId",
                table: "Agreement_File");

            migrationBuilder.DropIndex(
                name: "IX_Agreement_File_UserId",
                table: "Agreement_File");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Agreement_File");

            migrationBuilder.DropColumn(
                name: "size",
                table: "Agreement_File");

            migrationBuilder.DropColumn(
                name: "upload_date",
                table: "Agreement_File");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Agreement_File");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 10, 12, 39, 59, 939, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 15, 13, 36, 22, 688, DateTimeKind.Local));
        }
    }
}
