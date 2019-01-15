using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ChangeSizeAgreementFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 15, 15, 18, 14, 667, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 15, 13, 36, 22, 688, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "size",
                table: "Agreement_File",
                maxLength: 12,
                nullable: false,
                defaultValue: "0",
                oldClrType: typeof(string),
                oldMaxLength: 6,
                oldDefaultValue: "0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 15, 13, 36, 22, 688, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 1, 15, 15, 18, 14, 667, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "size",
                table: "Agreement_File",
                maxLength: 6,
                nullable: false,
                defaultValue: "0",
                oldClrType: typeof(string),
                oldMaxLength: 12,
                oldDefaultValue: "0");
        }
    }
}
