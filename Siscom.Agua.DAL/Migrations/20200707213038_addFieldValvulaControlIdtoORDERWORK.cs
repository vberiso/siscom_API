using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFieldValvulaControlIdtoORDERWORK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValvulaControlId",
                table: "Order_work",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 7, 16, 30, 36, 900, DateTimeKind.Local).AddTicks(9823),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 2, 17, 56, 47, 879, DateTimeKind.Local).AddTicks(7899));

            migrationBuilder.CreateIndex(
                name: "IX_Order_work_ValvulaControlId",
                table: "Order_work",
                column: "ValvulaControlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_work_valvula_control_ValvulaControlId",
                table: "Order_work",
                column: "ValvulaControlId",
                principalTable: "valvula_control",
                principalColumn: "id_valvula_control",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_work_valvula_control_ValvulaControlId",
                table: "Order_work");

            migrationBuilder.DropIndex(
                name: "IX_Order_work_ValvulaControlId",
                table: "Order_work");

            migrationBuilder.DropColumn(
                name: "ValvulaControlId",
                table: "Order_work");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 2, 17, 56, 47, 879, DateTimeKind.Local).AddTicks(7899),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 7, 16, 30, 36, 900, DateTimeKind.Local).AddTicks(9823));
        }
    }
}
