using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class DebtPrepaid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debt_Prepaid_Prepaid_PrepaidId",
                table: "Debt_Prepaid");

            migrationBuilder.DropIndex(
                name: "IX_Debt_Prepaid_PrepaidId",
                table: "Debt_Prepaid");

            migrationBuilder.DropColumn(
                name: "PrepaidId",
                table: "Debt_Prepaid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 27, 11, 57, 19, 818, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 27, 10, 47, 20, 782, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 27, 10, 47, 20, 782, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 27, 11, 57, 19, 818, DateTimeKind.Local));

            migrationBuilder.AddColumn<int>(
                name: "PrepaidId",
                table: "Debt_Prepaid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Debt_Prepaid_PrepaidId",
                table: "Debt_Prepaid",
                column: "PrepaidId");

            migrationBuilder.AddForeignKey(
                name: "FK_Debt_Prepaid_Prepaid_PrepaidId",
                table: "Debt_Prepaid",
                column: "PrepaidId",
                principalTable: "Prepaid",
                principalColumn: "id_prepaid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
