using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AgreementLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 13, 12, 11, 42, 416, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 13, 10, 44, 43, 888, DateTimeKind.Local));

            migrationBuilder.AddColumn<DateTime>(
                name: "end_date",
                table: "Discount",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "in_agreement",
                table: "Discount",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "Discount",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "new_value",
                table: "Agreement_Log",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "old_value",
                table: "Agreement_Log",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "visible",
                table: "Agreement_Log",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_date",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "in_agreement",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "Discount");

            migrationBuilder.DropColumn(
                name: "new_value",
                table: "Agreement_Log");

            migrationBuilder.DropColumn(
                name: "old_value",
                table: "Agreement_Log");

            migrationBuilder.DropColumn(
                name: "visible",
                table: "Agreement_Log");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 13, 10, 44, 43, 888, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 13, 12, 11, 42, 416, DateTimeKind.Local));
        }
    }
}
