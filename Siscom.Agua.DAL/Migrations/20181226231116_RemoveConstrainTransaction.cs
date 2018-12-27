using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class RemoveConstrainTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Pay_Method_PayMethodId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_PayMethodId",
                table: "Transaction");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 26, 17, 11, 15, 780, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 21, 10, 41, 56, 528, DateTimeKind.Local));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2018, 12, 21, 10, 41, 56, 528, DateTimeKind.Local),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2018, 12, 26, 17, 11, 15, 780, DateTimeKind.Local));

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PayMethodId",
                table: "Transaction",
                column: "PayMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Pay_Method_PayMethodId",
                table: "Transaction",
                column: "PayMethodId",
                principalTable: "Pay_Method",
                principalColumn: "id_pay_method",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
