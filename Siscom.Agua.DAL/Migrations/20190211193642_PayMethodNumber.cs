using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class PayMethodNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pay_method_number",
                table: "Transaction",
                maxLength: 31,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pay_method_number",
                table: "Payment",
                maxLength: 31,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 11, 13, 36, 41, 310, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 7, 13, 10, 20, 923, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pay_method_number",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "pay_method_number",
                table: "Payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 2, 7, 13, 10, 20, 923, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 2, 11, 13, 36, 41, 310, DateTimeKind.Local));
        }
    }
}
