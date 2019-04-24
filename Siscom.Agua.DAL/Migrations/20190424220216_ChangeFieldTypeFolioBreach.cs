using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ChangeFieldTypeFolioBreach : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 24, 17, 2, 15, 197, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 22, 18, 5, 56, 529, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "folio",
                table: "Breach",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 4, 22, 18, 5, 56, 529, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 4, 24, 17, 2, 15, 197, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "folio",
                table: "Breach",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 25);
        }
    }
}
