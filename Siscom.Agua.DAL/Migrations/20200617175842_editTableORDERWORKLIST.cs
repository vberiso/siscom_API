using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siscom.Agua.DAL.Migrations
{
    public partial class editTableORDERWORKLIST : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type_order",
                table: "order_work_list",
                newName: "type_order_result");

            migrationBuilder.RenameColumn(
                name: "folio_order",
                table: "order_work_list",
                newName: "folio_order_result");

            migrationBuilder.AddColumn<string>(
                name: "order_work_id_result",
                table: "order_work_list",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 17, 12, 58, 41, 89, DateTimeKind.Local).AddTicks(9096),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 16, 17, 7, 26, 752, DateTimeKind.Local).AddTicks(3624));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order_work_id_result",
                table: "order_work_list");

            migrationBuilder.RenameColumn(
                name: "type_order_result",
                table: "order_work_list",
                newName: "type_order");

            migrationBuilder.RenameColumn(
                name: "folio_order_result",
                table: "order_work_list",
                newName: "folio_order");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date_current",
                table: "Folio",
                nullable: false,
                defaultValue: new DateTime(2020, 6, 16, 17, 7, 26, 752, DateTimeKind.Local).AddTicks(3624),
                oldClrType: typeof(DateTime),
                oldDefaultValue: new DateTime(2020, 6, 17, 12, 58, 41, 89, DateTimeKind.Local).AddTicks(9096));
        }
    }
}
