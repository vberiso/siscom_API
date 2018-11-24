using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ChangeFieldAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 21, 12, 19, 18, 634, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 21, 11, 24, 19, 194, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "Lon",
                table: "Address",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "lat",
                table: "Address",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "current_date",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 11, 21, 11, 24, 19, 194, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 11, 21, 12, 19, 18, 634, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "Lon",
                table: "Address",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "lat",
                table: "Address",
                maxLength: 12,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
