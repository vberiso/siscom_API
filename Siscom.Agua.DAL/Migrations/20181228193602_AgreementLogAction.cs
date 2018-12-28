using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AgreementLogAction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 28, 13, 36, 1, 159, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 27, 20, 20, 13, 888, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "action",
                table: "Agreement_Log",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "controller",
                table: "Agreement_Log",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "action",
                table: "Agreement_Log");

            migrationBuilder.DropColumn(
                name: "controller",
                table: "Agreement_Log");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 27, 20, 20, 13, 888, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 28, 13, 36, 1, 159, DateTimeKind.Local));
        }
    }
}
