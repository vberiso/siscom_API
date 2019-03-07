using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class OrderSaleDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "Tariff_Product",
                maxLength: 5,
                nullable: false,
                defaultValue: "TTP01");

            migrationBuilder.AlterColumn<decimal>(
                name: "quantity",
                table: "Order_Sale_Detail",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 7, 16, 59, 8, 915, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 7, 15, 7, 22, 560, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "Tariff_Product");

            migrationBuilder.AlterColumn<short>(
                name: "quantity",
                table: "Order_Sale_Detail",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 3, 7, 15, 7, 22, 560, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 3, 7, 16, 59, 8, 915, DateTimeKind.Local));
        }
    }
}
