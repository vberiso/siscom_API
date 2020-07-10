using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class addFierdValvulaControlIdToORDERWORD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValvulaControlId",
                table: "Order_work",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 7, 10, 15, 52, 3, 969, DateTimeKind.Local).AddTicks(4087),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 10, 15, 47, 59, 530, DateTimeKind.Local).AddTicks(3290));

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2020, 7, 10, 15, 52, 4, 46, DateTimeKind.Local).AddTicks(157));

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
                defaultValue: new DateTime(2020, 7, 10, 15, 47, 59, 530, DateTimeKind.Local).AddTicks(3290),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 7, 10, 15, 52, 3, 969, DateTimeKind.Local).AddTicks(4087));

            migrationBuilder.UpdateData(
                table: "valvula_control",
                keyColumn: "id_valvula_control",
                keyValue: 1,
                column: "last_service_date",
                value: new DateTime(2020, 7, 10, 15, 47, 59, 654, DateTimeKind.Local).AddTicks(5568));
        }
    }
}
