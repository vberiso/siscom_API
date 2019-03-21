using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addAccountPaymentTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "account",
                table: "Transaction",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "account",
                table: "Payment",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 21, 11, 39, 1, 463, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 21, 10, 17, 40, 996, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "route",
                table: "Agreement",
                maxLength: 50,
                nullable: false,
                defaultValue: "0",
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "account",
                table: "Payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 21, 10, 17, 40, 996, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 21, 11, 39, 1, 463, DateTimeKind.Local));

            migrationBuilder.AlterColumn<string>(
                name: "route",
                table: "Agreement",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldDefaultValue: "0");
        }
    }
}
