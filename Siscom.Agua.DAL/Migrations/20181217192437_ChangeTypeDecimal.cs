using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ChangeTypeDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Transaction_Detail",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "total",
                table: "Transaction",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "tax",
                table: "Transaction",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "rounding",
                table: "Transaction",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Transaction",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "cash_box",
                table: "Terminal",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Tariff_Product",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "start_consume",
                table: "Tariff",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "end_consume",
                table: "Tariff",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Tariff",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "number_column",
                table: "System_Parameters",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Prepaid_Detail",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Prepaid",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "accredited",
                table: "Prepaid",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Payment_Detail",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "total",
                table: "Payment",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "tax",
                table: "Payment",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "subtotal",
                table: "Payment",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "rounding",
                table: "Payment",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "total",
                table: "Notification",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "tax",
                table: "Notification",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "subtotal",
                table: "Notification",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "rounding",
                table: "Notification",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 17, 13, 24, 37, 392, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 14, 11, 55, 47, 676, DateTimeKind.Local));

            migrationBuilder.AlterColumn<decimal>(
                name: "payment_amount",
                table: "Debt_Prepaid",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "original_amount",
                table: "Debt_Prepaid",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "original_amount",
                table: "Debt_Discount",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_amount",
                table: "Debt_Discount",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "on_account",
                table: "Debt_Detail",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Debt_Detail",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "on_account",
                table: "Debt",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "Debt",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "consumption",
                table: "Consumption",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "previous_consumption",
                table: "Consumption",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "current_consumption",
                table: "Consumption",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Transaction_Detail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "total",
                table: "Transaction",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "tax",
                table: "Transaction",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "rounding",
                table: "Transaction",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Transaction",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "cash_box",
                table: "Terminal",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Tariff_Product",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "start_consume",
                table: "Tariff",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "end_consume",
                table: "Tariff",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Tariff",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "number_column",
                table: "System_Parameters",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Prepaid_Detail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Prepaid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "accredited",
                table: "Prepaid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Payment_Detail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "total",
                table: "Payment",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "tax",
                table: "Payment",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "subtotal",
                table: "Payment",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "rounding",
                table: "Payment",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "total",
                table: "Notification",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "tax",
                table: "Notification",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "subtotal",
                table: "Notification",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "rounding",
                table: "Notification",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 14, 11, 55, 47, 676, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 17, 13, 24, 37, 392, DateTimeKind.Local));

            migrationBuilder.AlterColumn<double>(
                name: "payment_amount",
                table: "Debt_Prepaid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "original_amount",
                table: "Debt_Prepaid",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "original_amount",
                table: "Debt_Discount",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "discount_amount",
                table: "Debt_Discount",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "on_account",
                table: "Debt_Detail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Debt_Detail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "on_account",
                table: "Debt",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "Debt",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "consumption",
                table: "Consumption",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "previous_consumption",
                table: "Consumption",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<double>(
                name: "current_consumption",
                table: "Consumption",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");
        }
    }
}
