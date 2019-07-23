using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldAspUserSuburbAccountingPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "settlement",
                table: "Suburb",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zip",
                table: "Suburb",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zone",
                table: "Suburb",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 7, 23, 13, 22, 45, 900, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 7, 16, 12, 11, 26, 527, DateTimeKind.Local));

            migrationBuilder.AddColumn<string>(
                name: "serial",
                table: "AspNetUsers",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idSolPago",
                table: "accounting_payment",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "settlement",
                table: "Suburb");

            migrationBuilder.DropColumn(
                name: "zip",
                table: "Suburb");

            migrationBuilder.DropColumn(
                name: "zone",
                table: "Suburb");

            migrationBuilder.DropColumn(
                name: "serial",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "idSolPago",
                table: "accounting_payment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2019, 7, 16, 12, 11, 26, 527, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2019, 7, 23, 13, 22, 45, 900, DateTimeKind.Local));
        }
    }
}
