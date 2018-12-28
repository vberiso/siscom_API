using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class ChangeTypeDebCalculation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 27, 20, 20, 13, 888, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 27, 20, 10, 57, 926, DateTimeKind.Local));

            migrationBuilder.AlterColumn<decimal>(
                name: "factor",
                table: "Debt_Calculation",
                nullable: false,
                oldClrType: typeof(short));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 27, 20, 10, 57, 926, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 27, 20, 20, 13, 888, DateTimeKind.Local));

            migrationBuilder.AlterColumn<short>(
                name: "factor",
                table: "Debt_Calculation",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
