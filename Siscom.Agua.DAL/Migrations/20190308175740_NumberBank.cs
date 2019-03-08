using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class NumberBank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "account_number",
                table: "Transaction",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "number_bank",
                table: "Transaction",
                maxLength: 33,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "account_number",
                table: "Payment",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "number_bank",
                table: "Payment",
                maxLength: 33,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 8, 11, 57, 40, 116, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 7, 16, 59, 8, 915, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account_number",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "number_bank",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "account_number",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "number_bank",
                table: "Payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 7, 16, 59, 8, 915, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 8, 11, 57, 40, 116, DateTimeKind.Local));
        }
    }
}
