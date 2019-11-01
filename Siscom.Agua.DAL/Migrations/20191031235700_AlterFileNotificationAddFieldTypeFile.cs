using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AlterFileNotificationAddFieldTypeFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type_file",
                table: "notification_files",
                nullable: true,
                defaultValue: "FI001");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 31, 17, 56, 59, 860, DateTimeKind.Local).AddTicks(7688),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 31, 15, 35, 59, 149, DateTimeKind.Local).AddTicks(6553));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type_file",
                table: "notification_files");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 31, 15, 35, 59, 149, DateTimeKind.Local).AddTicks(6553),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 31, 17, 56, 59, 860, DateTimeKind.Local).AddTicks(7688));
        }
    }
}
