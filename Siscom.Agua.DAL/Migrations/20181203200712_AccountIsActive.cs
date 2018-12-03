using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class AccountIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 3, 14, 7, 11, 396, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 3, 14, 1, 23, 701, DateTimeKind.Local));

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "Account",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 3, 14, 1, 23, 701, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 3, 14, 7, 11, 396, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "is_active",
                table: "Account",
                nullable: false,
                oldClrType: typeof(bool));
        }
    }
}
