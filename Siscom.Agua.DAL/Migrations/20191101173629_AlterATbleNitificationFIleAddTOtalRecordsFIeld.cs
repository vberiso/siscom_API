using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AlterATbleNitificationFIleAddTOtalRecordsFIeld : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "total_records",
                table: "notification_files",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 1, 11, 36, 29, 72, DateTimeKind.Local).AddTicks(3743),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 1, 9, 58, 48, 334, DateTimeKind.Local).AddTicks(8256));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total_records",
                table: "notification_files");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 1, 9, 58, 48, 334, DateTimeKind.Local).AddTicks(8256),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 1, 11, 36, 29, 72, DateTimeKind.Local).AddTicks(3743));
        }
    }
}
