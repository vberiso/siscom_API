using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFielAccountingAccountTableAccountingCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 28, 14, 49, 51, 356, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 27, 18, 57, 56, 121, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "accounting_account",
                table: "accounting_code",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accounting_account",
                table: "accounting_code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 27, 18, 57, 56, 121, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 28, 14, 49, 51, 356, DateTimeKind.Local));
        }
    }
}
