using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DebtFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 6, 14, 8, 54, 45, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 6, 14, 0, 9, 584, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "DebtPeriodId",
                table: "Debt",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 6, 14, 0, 9, 584, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 6, 14, 8, 54, 45, DateTimeKind.Local));

            migrationBuilder.AlterColumn<int>(
                name: "DebtPeriodId",
                table: "Debt",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
