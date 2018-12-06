using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DebtPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debt_Debt_Period_DebtPeriodId",
                table: "Debt");

            migrationBuilder.DropIndex(
                name: "IX_Debt_DebtPeriodId",
                table: "Debt");

            migrationBuilder.DropColumn(
                name: "TypePeriodId",
                table: "Discount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 6, 14, 0, 9, 584, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 6, 11, 53, 25, 483, DateTimeKind.Local));

            migrationBuilder.AddColumn<short>(
                name: "month",
                table: "Discount",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "month",
                table: "Discount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 6, 11, 53, 25, 483, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 6, 14, 0, 9, 584, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "TypePeriodId",
                table: "Discount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Debt_DebtPeriodId",
                table: "Debt",
                column: "DebtPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Debt_Debt_Period_DebtPeriodId",
                table: "Debt",
                column: "DebtPeriodId",
                principalTable: "Debt_Period",
                principalColumn: "id_debt_period",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
