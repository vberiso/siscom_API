using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AlterATbleNitificationFIleAddFolioFIeld : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "folio",
                table: "notification_files",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 11, 1, 9, 58, 48, 334, DateTimeKind.Local).AddTicks(8256),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 10, 31, 17, 56, 59, 860, DateTimeKind.Local).AddTicks(7688));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "folio",
                table: "notification_files");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 10, 31, 17, 56, 59, 860, DateTimeKind.Local).AddTicks(7688),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 11, 1, 9, 58, 48, 334, DateTimeKind.Local).AddTicks(8256));
        }
    }
}
