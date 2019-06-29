using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFielAccountingAccountTableAccountingPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 28, 15, 57, 55, 687, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 28, 14, 49, 51, 356, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "accounting_account",
                table: "accounting_payment",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "accounting_account",
                table: "accounting_payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 6, 28, 14, 49, 51, 356, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 6, 28, 15, 57, 55, 687, DateTimeKind.Local));
        }
    }
}
